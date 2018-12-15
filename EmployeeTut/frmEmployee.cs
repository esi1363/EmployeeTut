using NHibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EmployeeTut
{
    public partial class frmEmployee : Form
    {
        public frmEmployee()
        {
            InitializeComponent();
        }

        private void frmEmployee_Load(object sender, EventArgs e)
        {
            LoadEmployeeData();
        }

        private void LoadEmployeeData()
        {
            ISession session = SessionFactory.OpenSession;
            using (session)
            {
                IQuery query = session.CreateQuery("From Employee");
                IList<Employee> empInfo = query.List<Employee>();
                dgvEmployee.DataSource = empInfo;
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            LoadEmployeeData();
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            Employee empData = new Employee();

            SetEmployeeData(empData);
            ISession session = SessionFactory.OpenSession;
            using (session)
            {
                using (ITransaction trans = session.BeginTransaction())
                {
                    try
                    {
                        session.Save(empData);
                        trans.Commit();
                        LoadEmployeeData();

                        resetDataControls();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        MessageBox.Show(ex.Message);
                        throw ex;
                    }
                }
            }

        }

        private void resetDataControls()
        {
            tbEmail.Clear();
            tbFirstName.Clear();
            tbLastName.Clear();
            tbId.Clear();
            tbFirstName.Focus();
        }

        private void SetEmployeeData(Employee emp)
        {
            //emp.Id = Int32.Parse(tbId.Text);
            emp.FirstName = tbFirstName.Text;
            emp.LastName = tbLastName.Text;
            emp.Email = tbEmail.Text;
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            //to update data we will load current data to our textbox and then update
            ISession session = SessionFactory.OpenSession;

            using (session)
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    try
                    {
                        IQuery query = session.CreateQuery("FROM Employee WHERE Id = '" + tbId.Text + "'");
                        Employee empData = query.List<Employee>()[0];
                        SetEmployeeData(empData);
                        session.Update(empData);
                        transaction.Commit();

                        LoadEmployeeData();

                        resetDataControls();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            ISession session = SessionFactory.OpenSession;
            string id = (tbId.Text != "") ? tbId.Text : null;
            using (session)
            {
                using (ITransaction trans = session.BeginTransaction())
                {
                    try
                    {
                        Employee emp = (Employee)session.Load(typeof(Employee), Int32.Parse(id.Trim()));
                        session.Delete(emp);
                        trans.Commit();
                        LoadEmployeeData();
                        resetDataControls();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        MessageBox.Show(ex.Message);
                        throw ex;
                    }
                }
            }
        }

        private void dgvEmployee_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private IList<Employee> GetEmployeeDataById(string id)
        {
            ISession session = SessionFactory.OpenSession;

            using (session)
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    try
                    {
                        IQuery query = session.CreateQuery("FROM Employee WHERE Id = '" + id + "'");
                        return query.List<Employee>();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        throw ex;
                    }
                }
            }
        }

        private void dgvEmployee_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvEmployee.RowCount <= 1 || e.RowIndex < 0)
                return;
            string id = dgvEmployee[0, e.RowIndex].Value.ToString();

            if (id == "")
                return;

            IList<Employee> empInfo = GetEmployeeDataById(id);

            tbId.Text = empInfo[0].Id.ToString();
            tbFirstName.Text = empInfo[0].FirstName.ToString();
            tbLastName.Text = empInfo[0].LastName.ToString();
            tbEmail.Text = empInfo[0].Email.ToString();
        }
    }
}
