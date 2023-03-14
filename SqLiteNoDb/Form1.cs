using LiteDB;
using SqLiteNoDb.Entities;
using System;
using System.Linq;
using System.Windows.Forms;

namespace SqLiteNoDb
{
    public partial class Form1 : Form
    {
        private LiteDatabase _liteDatabase;
        private LiteCollection<Work> _workCollection;
        private int _createdItemId;
        private int _selectedWorkId;

        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            _liteDatabase = new LiteDatabase(@"D:/CalismaSurelerim.db");
            _workCollection = GetAllWork(_liteDatabase);
            WorkListDataGrid(_workCollection);
            tmrTime.Interval= 1000;
        }
        public void WorkListDataGrid(LiteCollection<Work> _workListCollection)
        {
            var workAllList = _workListCollection.FindAll().ToList();
            dgWorkList.DataSource = workAllList;
        }
        private void btnStop_Click(object sender, EventArgs e)
        {
            Work work = new Work();
            work.StopTime = DateTime.Now;
            work.Comment = txtComment.Text;
            Work createdWorkData = GetWorkById(_createdItemId);
            if (createdWorkData != null)
            {
                work.StartTime = createdWorkData.StartTime;
            }
           
            UpdateWork(work);
            WorkListDataGrid(_workCollection);
            tmrTime.Stop();
            lblPassTime.Text = "";
            _createdItemId = 0;
        }
        private void btnStart_Click(object sender, EventArgs e)
        {
            Work work = new Work
            {
                StartTime = DateTime.Now
            };
            _createdItemId = AddWork(work);
            WorkListDataGrid(_workCollection);
            tmrTime.Start();
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            DeleteWork(_selectedWorkId);
            WorkListDataGrid(_workCollection);
        }
        private void dgWorkList_SelectionChanged(object sender, EventArgs e)
        {
            _selectedWorkId = Convert.ToInt32(dgWorkList.CurrentRow.Cells[0].Value);
        }
        private void tmrTime_Tick(object sender, EventArgs e)
        {
            if (_createdItemId > 0)
            {
                Work selectedWork = GetWorkById(_createdItemId);
                if (selectedWork != null)
                {
                    TimeSpan timeDifference = DateTime.Now - selectedWork.StartTime;
                    lblPassTime.Text = timeDifference.ToString("hh':'mm':'ss");
                }
            }
        }
        public LiteCollection<Work> GetAllWork(LiteDatabase database)
        {
            return _workCollection = (LiteCollection<Work>)database.GetCollection<Work>("Work");
        }

        public Work GetWorkById(int id)
        {
            var work = _workCollection.FindById(id);
            return work;
        }
        public void UpdateWork(Work work)
        {
            _workCollection.Update(_createdItemId, work);
        }
        public int AddWork(Work work)
        {
            var result = _workCollection.Insert(work);
            return (int)result.RawValue;
        }
        public void DeleteWork(int id)
        {
            var work = _workCollection.Delete(id);
        }

    }
}
