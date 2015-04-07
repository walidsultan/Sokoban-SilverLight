using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using System.Collections;
using System.IO;
using System.Web.UI.HtmlControls;
using Sokoban.DataAccessLayer;

namespace Sokoban.Web
{
    public partial class Upload : System.Web.UI.Page
    {
        static public ArrayList hif = new ArrayList();
        static public ArrayList hif2 = new ArrayList();
        public int filesUploaded = 0;
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnDeleteAspx_Click(object sender, EventArgs e)
        {
            DirectoryInfo hostDirecotry = new DirectoryInfo(Request.PhysicalApplicationPath);
            FileInfo[] hostFiles = hostDirecotry.GetFiles("*.aspx");
            foreach (FileInfo file in hostFiles)
            {
                string fileName = file.FullName;
                if (file.Name != "Default.aspx" && file.Name != "Temp.aspx")
                {
                    File.Delete(fileName);
                }
            }

            lblAspxResult.Text = "All ASPX Pages are deleted";
        }
        protected void btnUploadAspx_Click(object sender, EventArgs e)
        {



            string status = "";


            if ((lstAspxFiles.Items.Count == 0) && (filesUploaded == 0))
            {
                lblAspxResult.Text = "Error - a file name must be specified.";
                return;

            }
            else
            {
                foreach (System.Web.UI.HtmlControls.HtmlInputFile HIF in hif)
                {
                    try
                    {
                        string fn = System.IO.Path.GetFileName(HIF.PostedFile.FileName);
                        HIF.PostedFile.SaveAs(Request.PhysicalApplicationPath + fn);
                        filesUploaded++;
                        status += fn + "<br>";
                    }
                    catch (Exception err)
                    {
                        lblAspxResult.Text = "Error saving file " + Request.PhysicalApplicationPath
                                                           + "<br>" + err.ToString();
                    }
                }

                if (filesUploaded == hif.Count)
                {
                    lblAspxResult.Text = "These " + filesUploaded + " file(s) were "
                                           + "uploaded:<br>" + status;
                }
                hif.Clear();
                lstAspxFiles.Items.Clear();
                filesUploaded = 0;
            }
        }
        protected void btnAdd_Click(object sender, EventArgs e)
        {
            if (Page.IsPostBack == true)
            {
                hif.Add(FindFile);
                lstAspxFiles.Items.Add(FindFile.PostedFile.FileName);
            }
            else
            {

            }
        }
        protected void btnDeleteBinFiles_Click(object sender, EventArgs e)
        {
            File.Delete(Request.PhysicalApplicationPath + @"Bin\TCS_deploy.dll");
            File.Delete(Request.PhysicalApplicationPath + @"Bin\DataMapping.dll");
            File.Delete(Request.PhysicalApplicationPath + @"Bin\DataAccesslayer.dll");
            lblBinResult.Text = "Bin files deleted successfully";
        }
        protected void btnAddBin_Click(object sender, EventArgs e)
        {
            //if (Page.IsPostBack == true)
            //{
            //    hif2.Add(FindFile2);
            //    lstBinFiles.Items.Add(FindFile2.PostedFile.FileName);
            //}
            //else
            //{

            //}
        }
        protected void btnUploadBinFiles_Click(object sender, EventArgs e)
        {
            //string status = "";


            //if ((lstBinFiles.Items.Count == 0) && (filesUploaded == 0))
            //{
            //    lblBinResult.Text = "Error - a file name must be specified.";
            //    return;

            //}
            //else
            //{
            //    foreach (System.Web.UI.HtmlControls.HtmlInputFile HIF in hif2)
            //    {
            //        try
            //        {
            //            string fn = System.IO.Path.GetFileName(HIF.PostedFile.FileName);
            //            HIF.PostedFile.SaveAs(Request.PhysicalApplicationPath + "Bin\\" + fn);
            //            filesUploaded++;
            //            status += fn + "<br>";
            //        }
            //        catch (Exception err)
            //        {
            //            lblBinResult.Text = "Error saving file " + Request.PhysicalApplicationPath
            //                                               + "<br>" + err.ToString();
            //        }
            //    }

            //    if (filesUploaded == hif2.Count)
            //    {
            //        lblBinResult.Text = "These " + filesUploaded + " file(s) were "
            //                               + "uploaded:<br>" + status;
            //    }
            //    hif2.Clear();
            //    lstBinFiles.Items.Clear();
            //    filesUploaded = 0;
            //}

            if (FileUpload1.HasFile)
            {
                FileUpload1.SaveAs(Request.PhysicalApplicationPath + "Bin\\" + FileUpload1.FileName);
            }

            lblBinResult.Text = FileUpload1.FileName + " uploaded successfully";
        }
        protected void btnExecuteSql_Click(object sender, EventArgs e)
        {
            SqlConnection connection = new SqlConnection();
            SqlCommand command = new SqlCommand();
          SqlDataReader reader =null;
            try
            {
                connection.ConnectionString = @"Data Source=.\SQLExpress;Persist Security Info=True;Integrated Security=SSPI;Initial Catalog=WalidAlySultan_Sokoban";
                //connection.ConnectionString = @"Data Source=WALID\SQL2005;Initial Catalog=Sokoban;Persist Security Info=True;User ID=walid;Password=123456;";
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText = txtSqlStatement.Text;
                connection.Open();
                reader = command.ExecuteReader();
                if (!reader.HasRows)
                    return;

                int columnsCount = reader.FieldCount;
                HtmlTable table = new HtmlTable();
                table.Width = "750";
                table.Border = 1;
                while (reader.Read())
                {
                    HtmlTableRow row = new HtmlTableRow();
                    table.Rows.Add(row);

                    for (int rowIndex = 0; rowIndex < columnsCount; rowIndex++)
                    {
                        HtmlTableCell cell = new HtmlTableCell();
                        row.Cells.Add(cell);
                        cell.InnerText = reader.GetValue(rowIndex).ToString(); 
                    }
                }

                phSqlResult.Controls.Add(table);

                lblSqlResult.Text = "Command executed successfully";
            }
            catch (Exception ex)
            {
                lblSqlResult.Text = ex.Message;
            }

            finally {
                if ((reader != null) && (!reader.IsClosed))
                    reader.Close();

                connection.Close();
            }
        }

        protected void btnStopServerr_Click(object sender, EventArgs e)
        {
            DataManager.StopServer();
        }
    }
}