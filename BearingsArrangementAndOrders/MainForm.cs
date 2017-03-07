using System;
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

            ExcelInterface curExcel = new ExcelInterface("C:\\Users\\zamyckijj-ei\\Google Диск\\Работа\\Комплектовка ПШ\\комплектовка суточного.xlsx");
            curExcel.LoadItemTypesFromExcel(curArranger.ItemTypes);
            curExcel.LoadBearingTypesFromExcel(curArranger.BearingTypes, curArranger.ItemTypes);
            curExcel.LoadBearingArrangementOrderFromExcel(curArranger.BearingArrOrders, curArranger.BearingTypes);
            curExcel.LoadBearingItemsGroupsFromExcel(curArranger.ItemsGroups, curArranger.ItemTypes);

            curArranger.DoArrangement();

            curExcel.ArrOrdersResultOutput(curArranger.BearingArrOrders, curArranger.ItemsGroups);

        }
    }
}
