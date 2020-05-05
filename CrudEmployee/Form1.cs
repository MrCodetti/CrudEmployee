using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace CrudEmployee
{
    public partial class frmEmployee : Form
    {
        string connString = (@"Data Source=DESKTOP-0S31MOV\SQLKURS;" +
                             "Initial Catalog=dbKundenVerwaltung;Integrated Security=sspi");

        SqlConnection sqlCon;
        SqlCommand sqlCmd;
        string EmployeeId = "";

        public frmEmployee()
        {
            InitializeComponent();
            sqlCon = new SqlConnection(connString);
            sqlCon.Open();
        }

        private void frmEmployee_Load(object sender, EventArgs e)
        {
            //dgvEmployee.AutoGenerateColumns = false;
            dgvEmployee.DataSource = FetchEmpDetails();
        }

        private DataTable FetchEmpDetails()
        {
            if (sqlCon.State == ConnectionState.Closed)
            {
                sqlCon.Open();
            }
            DataTable dtData = new DataTable();
            sqlCmd = new SqlCommand("uspEployee", sqlCon);
            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.Parameters.AddWithValue("@ActionType", "FetchData");
            SqlDataAdapter sqlSda = new SqlDataAdapter(sqlCmd);
            sqlSda.Fill(dtData);
            return dtData;
        }

        private DataTable FetchEmpRecords(string empId)
        {
            if (sqlCon.State == ConnectionState.Closed)
            {
                sqlCon.Open();
            }
            DataTable sqlDt = new DataTable();
            sqlCmd = new SqlCommand("uspEployee", sqlCon);
            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.Parameters.AddWithValue("@ActionType", "FetchRecord");
            sqlCmd.Parameters.AddWithValue("@EmpId", empId);
            SqlDataAdapter sqlDa = new SqlDataAdapter(sqlCmd);
            sqlDa.Fill(sqlDt);
            return sqlDt;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txbName.Text == "" && 
                txbCity.Text == "" && 
                txbDepartment.Text == "" &&
                cobGender.SelectedIndex <= -1)
            {
                MessageBox.Show("Bitte Keine freie felder!", "Syntax Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cobGender.Select();
            }
            else
            {
                try
                {
                    if (sqlCon.State == ConnectionState.Closed)
                    {
                        sqlCon.Open();
                    }
                    sqlCmd = new SqlCommand("uspEployee", sqlCon);
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Parameters.AddWithValue("@ActionType", "SaveData");
                    sqlCmd.Parameters.AddWithValue("@EmpId", EmployeeId);
                    sqlCmd.Parameters.AddWithValue("@Name", txbName.Text);
                    sqlCmd.Parameters.AddWithValue("@City", txbCity.Text);
                    sqlCmd.Parameters.AddWithValue("@Deparment", txbDepartment.Text);
                    sqlCmd.Parameters.AddWithValue("@Gender", cobGender.Text);
                    int numRes = sqlCmd.ExecuteNonQuery();
                    if (numRes > 0)
                    {
                        MessageBox.Show("Record Saved Successfully !!!");
                        ClearAllData();
                    }
                    else
                    {
                        MessageBox.Show("Try Again !!!");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearAllData();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (EmployeeId != "")
            {
                try
                {
                    if (sqlCon.State == ConnectionState.Closed)
                    {
                        sqlCon.Open();
                    }
                    DataTable sqlDt = new DataTable();
                    sqlCmd = new SqlCommand("uspEployee", sqlCon);
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Parameters.AddWithValue("@ActionType", "DeleteData");
                    sqlCmd.Parameters.AddWithValue("@EmpId", EmployeeId);
                    int numRes = sqlCmd.ExecuteNonQuery();
                    if (numRes > 0)
                    {
                        MessageBox.Show("Record Deleted Successfully !!!");
                        ClearAllData();
                    }
                    else
                    {
                        MessageBox.Show("Try Again !!!");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Please select a record !!!");
            }
        }

        private void dgvEmployee_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                btnSave.Text = "Update";
                EmployeeId = dgvEmployee.Rows[e.RowIndex].Cells[0].Value.ToString();
                DataTable DT = FetchEmpRecords(EmployeeId);
                if (DT.Rows.Count > 0)
                {
                    txbName.Text = DT.Rows[0][1].ToString();
                    txbCity.Text = DT.Rows[0][2].ToString();
                    txbDepartment.Text = DT.Rows[0][3].ToString();
                    cobGender.Text = DT.Rows[0][4].ToString();
                }
                else
                {
                    ClearAllData();
                }
            }
        }

        private void ClearAllData()
        {
            btnSave.Text = "Save";
            txbName.Clear();
            txbCity.Clear();
            txbDepartment.Clear();
            cobGender.SelectedIndex = -1;
            EmployeeId = "";
            dgvEmployee.AutoGenerateColumns = false;
            dgvEmployee.DataSource = FetchEmpDetails();
        }
    }
}
