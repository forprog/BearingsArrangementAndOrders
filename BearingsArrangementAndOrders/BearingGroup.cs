using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace BearingsArrangementAndOrders
{
    public class BearingGroup : IXmlSerializable
    {
        public BearingType Type;
        public SerializableDictionary<string, BearingItemsGroup> BearingItemsGroups = new SerializableDictionary<string, BearingItemsGroup>();
        public double Rad1()
        {
            return Type.Rad1Nominal.GetValueOrDefault() + BearingItemsGroups["01"].Size1 - BearingItemsGroups["02"].Size1 - 2 * BearingItemsGroups["04"].Size1;
        }

        public double Rad1Devation()
        {
            //todo проверить корректность
            return Math.Abs((Type.Rad1Max.GetValueOrDefault()-Type.Rad1Min.GetValueOrDefault())/2-Rad1());
        } 

        private int pCount;
        public int Count
        {
            get { return pCount; }
        }

        public  int GetCount()
        {
            int iBearingCount = int.MaxValue;
            foreach (var curKVPare in BearingItemsGroups)
            {
                var curItemGroup = curKVPare.Value;
                int iItemCount = Convert.ToInt32(Math.Floor(curItemGroup.ItemCount / Convert.ToDouble(Type.BearingItemsCount[curItemGroup.ItemType.Type])));
                iBearingCount = Math.Min(iBearingCount, iItemCount);
            }
            return iBearingCount;
        }

        public void SetCount(int paramCount)
        {
            pCount = paramCount;
        }

        public void SetCount()
        {
            pCount = GetCount();

        }

        public bool IsArrangement()
        {
            if ((Rad1() >= Type.Rad1Min) && (Rad1() <= Type.Rad1Max))
            { return true; }
            else
            { return false; }
        }

        public BearingGroup(BearingGroup paramBearingGroup)
        {
            Type = paramBearingGroup.Type;
            foreach (var item in paramBearingGroup.BearingItemsGroups)
            {
                BearingItemsGroups.Add(item.Key, item.Value);
            }
        }
        public BearingGroup()
        {
        }

        #region IXmlSerializable Members
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            XmlSerializer StringSerializer = new XmlSerializer(typeof(String));
            XmlSerializer DoubleSerializer = new XmlSerializer(typeof(Double));
            XmlSerializer IntSerializer = new XmlSerializer(typeof(int));

            writer.WriteStartElement("Rad1");
            DoubleSerializer.Serialize(writer, this.Rad1());
            writer.WriteEndElement();
            writer.WriteStartElement("Count");
            IntSerializer.Serialize(writer, this.Count);
            writer.WriteEndElement();

            foreach (var curItemGroup in this.BearingItemsGroups)
            {
                writer.WriteStartElement("BearingItem");
                writer.WriteStartElement("Item");
                StringSerializer.Serialize(writer, curItemGroup.Value.ItemType.Description);
                writer.WriteEndElement();
                //TODO поставить выгрузку характеристики
                writer.WriteStartElement("Characteristic");

                if (curItemGroup.Value.ItemType.Type=="04")
                {
                    StringSerializer.Serialize(writer, "ТУ");
                }
                else
                {
                    StringSerializer.Serialize(writer, "");
                }
                
                writer.WriteEndElement();
                writer.WriteStartElement("Count");
                IntSerializer.Serialize(writer, this.Count*this.Type.BearingItemsCount[curItemGroup.Value.ItemType.Type]);
                writer.WriteEndElement();
                writer.WriteStartElement("Size1");
                DoubleSerializer.Serialize(writer, curItemGroup.Value.Size1);
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
        }

        #endregion


        }
}

