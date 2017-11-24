﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BearingsArrangementAndOrders
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            BearingArranger curArranger = new BearingArranger();

            ExcelInterface curExcel = new ExcelInterface(tbFileName.Text);
            curExcel.LoadItemTypesFromExcel(curArranger.ItemTypes);
            curExcel.LoadBearingTypesFromExcel(curArranger.BearingTypes, curArranger.ItemTypes);
            curExcel.LoadBearingArrangementOrderFromExcel(curArranger.BearingArrOrders, curArranger.BearingTypes);
            curExcel.LoadBearingItemsGroupsFromExcel(curArranger.ItemsGroups, curArranger.ItemTypes);

            curArranger.DoArrangement();

            curExcel.ArrOrdersResultOutput(curArranger.BearingArrOrders, curArranger.ItemsGroups, curArranger.GrindingOrders);
            curExcel.Dispose();
            
            //сообщение о завершении комплектовки
            MessageBox.Show("Комплектовка завершена");

        }

        private void btFileNameSelect_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog()
            {

                // Set filter options and filter index.
                Filter = "Excel Files|*.xls;*.xlsm;*.xlsx",
                FilterIndex = 1,

                Multiselect = false
            };

            // Call the ShowDialog method to show the dialog box.
            var userClickedOK = openFileDialog1.ShowDialog();

            // Process input if the user clicked OK.
            if (userClickedOK == DialogResult.OK)
            {
                tbFileName.Text = openFileDialog1.FileName;
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            BearingArranger curArranger = new BearingArranger();

            ExcelInterface curExcel = new ExcelInterface(tbFileName.Text);
            curExcel.LoadItemTypesFromExcel(curArranger.ItemTypes);
            curExcel.LoadBearingTypesFromExcel(curArranger.BearingTypes, curArranger.ItemTypes);
            curExcel.LoadBearingArrangementOrderFromExcel(curArranger.BearingArrOrders, curArranger.BearingTypes);
            curExcel.LoadBearingItemsGroupsFromExcel(curArranger.ItemsGroups, curArranger.ItemTypes);
            curExcel.Dispose();

            curArranger.DoArrangement();

            XMLInterface curXML = new XMLInterface();
            curXML.ArrOrdersResultOutput(curArranger.BearingArrOrders, curArranger.ItemsGroups, curArranger.GrindingOrders);

            //сообщение о завершении комплектовки
            MessageBox.Show("Комплектовка завершена");

        }
    }
}
