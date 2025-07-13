using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace CoffeeCafeProject
{
    public partial class FrmMain : Form
    {

        //ตัวแปรสำหรับเก็บราคาเมนู
        float[] menuPrice = new float[10];

        //ตัวแปรเก็บรหัสสมาชิก
        int memberId = 0;

        public FrmMain()
        {
            InitializeComponent();
        }

        private void btMenu_Click(object sender, EventArgs e)
        {
            FrmMenu frmMenu = new FrmMenu();
            frmMenu.ShowDialog();
            resetForm();

        }

        private void btMember_Click(object sender, EventArgs e)
        {
            FrmMember frmMember = new FrmMember();
            frmMember.ShowDialog();
        }
        //เมธอด resetForm

        private void resetForm()
        {
            //ตั้งค่า memberId เป็น 0 
            memberId = 0;
            // ให้ rdMenberNo , rdMemberYes เป็น false ไม่ถูกเลือก
            rdMenberNo.Checked = false;
            rdMemberYes.Checked = false;
            // ให้tbMemberPhone เป็นค่าว่าง และใช้งานไม่ได้
            tbMemberPhone.Clear();
            tbMemberPhone.Enabled = false;
            // ให้ tbMemberName เป็นข้อความ ชื่อสมาชิก 
            tbMemberName.Text = "(ชื่อสมาชิก)";
            //ให้ tbMemberScore เป็น 0
            lbMemberScore.Text = "0";
            //ให้lbOrderPay เป็น 0.00
            lbOrderPay.Text = "0.00";
            //เคลียร์ lvOrederMenu
            lvOrderMenu.Items.Clear();
            lvOrderMenu.Columns.Clear();
            lvOrderMenu.FullRowSelect = true;
            lvOrderMenu.View = View.Details;
            lvOrderMenu.Columns.Add("ชื่อเมนู", 150, HorizontalAlignment.Left);
            lvOrderMenu.Columns.Add("ราคา", 80, HorizontalAlignment.Left);
            //ดึงข้อมูลรายการเมนูมาแสดงที่หน้าจอ และเก็บไว้ใช้กับตอนที่ ผู้ใช้เลือกสั่งเมนู
            //สร้างคอนเนคชั่นกับฐานข้อมูล
            using (SqlConnection sqlConnection = new SqlConnection(ShareResource.connectionString))
            {
                try
                {
                    sqlConnection.Open(); //เปิดการเชื่อมต่อฐานข้อมูล

                    //สร้างคำสั่ง SQL เพื่อดึงข้อมูลเมนู
                    string strSQL = "SELECT menuName, menuPrice,menuImage FROM menu_tb";

                    using (SqlDataAdapter dataAdapter = new SqlDataAdapter(strSQL, sqlConnection))
                    {
                        //เอาข้อมูลที่ได้จาก strSQL เป็นก้อนใน dataAdapter มาทำให้เป็นตารางโดยใส่ไว้ใน DataTable
                        DataTable dataTable = new DataTable();
                        dataAdapter.Fill(dataTable);

                        //สร้างตัวแปรอ้างถุึง PictureBox และ Button ที่จะเอารุปและเมนูไปแสดง 
                        PictureBox[] pbMenuImage = { pbMenu1, pbMenu2, pbMenu3, pbMenu4, pbMenu5, pbMenu6, pbMenu7, pbMenu8, pbMenu9, pbMenu10 };
                        Button[] btMenuName = { btMenu1, btMenu2, btMenu3, btMenu4, btMenu5, btMenu6, btMenu7, btMenu8, btMenu9, btMenu10 };

                        //ก่อน วนลูปใส่เข้าไปใหม่ ต้อง เคลียร์ข้อมูลใน PictureBox และ Button ก่อน ที่จะใส่ข้อมูลใหม่
                        for (int i = 0; i < pbMenuImage.Length; i++)
                        {
                            pbMenuImage[i].Image = CoffeeCafeProject.Properties.Resources.menu; // เคลียร์รูปใน PictureBox
                            btMenuName[i].Text = "Menu"; // เคลียร์ข้อความใน Button
                        }


                        //วนลูปเอาข้อมูลที่อยู่ในdatatable กำหนดให้กับ pbMenuImage , btMenuName ,MenuPrice เพื่อแสดงข้อมูลเมนูใน PictureBox และ Button
                        for (int i = 0; dataTable.Rows.Count < btMenuName.Length; i++)
                        {
                            btMenuName[i].Text = dataTable.Rows[i]["menuName"].ToString();
                            menuPrice[i] = float.Parse(dataTable.Rows[i]["menuPrice"].ToString());
                            //เอารูปไปกำหนดให้กับ PictureBox
                            if (dataTable.Rows[i]["menuImage"] != DBNull.Value)
                            {
                                //กรณีมีรูป ต้องแปลงข้อมูลจาก Binary เป็น Image
                                byte[] imgByte = (byte[])dataTable.Rows[i]["menuImage"];
                                using (var ms = new System.IO.MemoryStream(imgByte))
                                {
                                    pbMenuImage[i].Image = System.Drawing.Image.FromStream(ms);
                                }
                            }
                            else
                            {
                                pbMenuImage[i].Image = Properties.Resources.menu; // ถ้าไม่มีรูปให้เป็น null
                            }
                        }


                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("เกิดข้อผิดพลาดในการเชื่อมต่อฐานข้อมูล: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }


        private void FrmMain_Load(object sender, EventArgs e)
        {
            resetForm();
        }

        private void rdMenberNo_CheckedChanged(object sender, EventArgs e)
        {
            tbMemberPhone.Clear();
            tbMemberPhone.Enabled = false; //ไม่สามารถกรอกเบอร์โทรได้
            tbMemberName.Text = "(ชื่อสมาชิก)"; //ให้เป็นข้อความ ชื่อสมาชิก
            lbMemberScore.Text = "0"; //ให้คะแนนสมาชิกเป็น 0
            memberId = 0;
        }

        private void rdMemberYes_CheckedChanged(object sender, EventArgs e)
        {
            tbMemberPhone.Clear();
            tbMemberPhone.Enabled = true; //สามารถกรอกเบอร์โทรได้
            tbMemberName.Text = "(ชื่อสมาชิก)"; //ให้เป็นข้อความ ชื่อสมาชิก
            lbMemberScore.Text = "0"; //ให้คะแนนสมาชิกเป็น 0
        }

        private void tbMemberPhone_KeyUp(object sender, KeyEventArgs e)
        {
            //ตรวจสอบว่าปุ่มที่กดแล้วปล่อยใช่ปุ่ม Enter หรือไม่
            //ถัาไม่ใช่ก็ไม่ต้องทำอะไร แต่ถ้าใช่ ให้เอา เบอร์โทรไปค้นใน Database
            //แล้วชื่อกับ แต้ม มาโชว์ ส่วนรหัสเอาไว้ใช้บันทึกลง Database
            if (e.KeyCode == Keys.Enter)
            {
                //สร้างคอนเนคชั่นกับฐานข้อมูล
                using (SqlConnection sqlConnection = new SqlConnection(ShareResource.connectionString))
                {
                    try
                    {
                        sqlConnection.Open(); //เปิดการเชื่อมต่อฐานข้อมูล

                        //สร้างคำสั่ง SQL เพื่อดึงข้อมูลเมนู
                        string strSQL = "SELECT memberId, memberName, memberScore FROM member_tb WHERE memberPhone=@memberPhone";

                        using (SqlCommand sqlCommand = new SqlCommand(strSQL, sqlConnection))
                        {
                            sqlCommand.Parameters.Add("@memberPhone", SqlDbType.VarChar, 50).Value = tbMemberPhone.Text;

                            using (SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlCommand))
                            {
                                //เอาข้อมูลที่ได้จาก strSQL เป็นก้อนใน dataAdapter มาทำให้เป็นตารางโดยใส่ไว้ใน DataTable
                                DataTable dataTable = new DataTable();
                                dataAdapter.Fill(dataTable);

                                if (dataTable.Rows.Count == 1)
                                {
                                    tbMemberName.Text = dataTable.Rows[0]["memberName"].ToString();
                                    lbMemberScore.Text = dataTable.Rows[0]["memberScore"].ToString();
                                    memberId = int.Parse(dataTable.Rows[0]["memberId"].ToString());
                                }
                                else
                                {
                                    MessageBox.Show("เบอร์โทรนี้ไม่มี กรูณาป้อนเบอร์โทรใหม่อีกรอบ...!");
                                }
                            }

                        }
                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show("พบข้อผิดพลาด กรุณาลองใหม่ หรือ ติดต่อ IT : " + ex.Message);
                    }
                }
            }
        }

        private void btMenu1_Click(object sender, EventArgs e)
        {
            //ตรวจสอบว่าชื่อบนปุ่ม ต้องไม่ใช้คำว่า Menu (เพราะมันเป็น ดีฟอล ของ รูปที่ยังไม่ปรากฏ)
            //หากใช่ คำว่า Menu  ไม่ต้องทำอะไร   หากไม่ใช่คำว่า Menu แปลว่า มีรูปและชื่อ เมนูปรากฏ สามารถเลือกสั่งได้ 
            //หากไม่ใช่ ให้เอาชื่อเมนู กับ ราคา ไปใส่ใน lvOrderMenu แล้วบวกแต้มเพิ่ม และ บวกรวมเป็นเงินเพิ่ม
            if (btMenu1.Text != "Menu")
            {
                ListViewItem item = new ListViewItem(btMenu1.Text);
                item.SubItems.Add(menuPrice[0].ToString());
                lvOrderMenu.Items.Add(item);

                //บวกแต้มเพิ่ม ต้องตรวจสอบก่อนว่าเป็นสมาชิกไหม
                if (tbMemberName.Text != "(ชิ่อสมาชิก)")
                {
                    lbMemberScore.Text = (int.Parse(lbMemberScore.Text) + 1).ToString();
                }
                //บวกค่าเพิ่ม
                lbOrderPay.Text = (float.Parse(lbOrderPay.Text) + menuPrice[0]).ToString();

            }
        }

        private void btMenu2_Click(object sender, EventArgs e)
        {
            //ตรวจสอบว่าชื่อบนปุ่ม ต้องไม่ใช้คำว่า Menu (เพราะมันเป็น ดีฟอล ของ รูปที่ยังไม่ปรากฏ)
            //หากใช่ คำว่า Menu  ไม่ต้องทำอะไร   หากไม่ใช่คำว่า Menu แปลว่า มีรูปและชื่อ เมนูปรากฏ สามารถเลือกสั่งได้ 
            //หากไม่ใช่ ให้เอาชื่อเมนู กับ ราคา ไปใส่ใน lvOrderMenu แล้วบวกแต้มเพิ่ม และ บวกรวมเป็นเงินเพิ่ม
            if (btMenu2.Text != "Menu")
            {
                ListViewItem item = new ListViewItem(btMenu2.Text);
                item.SubItems.Add(menuPrice[1].ToString());
                lvOrderMenu.Items.Add(item);

                //บวกแต้มเพิ่ม ต้องตรวจสอบก่อนว่าเป็นสมาชิกไหม
                if (tbMemberName.Text != "(ชิ่อสมาชิก)")
                {
                    lbMemberScore.Text = (int.Parse(lbMemberScore.Text) + 1).ToString();
                }
                //บวกค่าเพิ่ม
                lbOrderPay.Text = (float.Parse(lbOrderPay.Text) + menuPrice[1]).ToString();
            }
        }

        private void btMenu3_Click(object sender, EventArgs e)
        {
            //ตรวจสอบว่าชื่อบนปุ่ม ต้องไม่ใช้คำว่า Menu (เพราะมันเป็น ดีฟอล ของ รูปที่ยังไม่ปรากฏ)
            //หากใช่ คำว่า Menu  ไม่ต้องทำอะไร   หากไม่ใช่คำว่า Menu แปลว่า มีรูปและชื่อ เมนูปรากฏ สามารถเลือกสั่งได้ 
            //หากไม่ใช่ ให้เอาชื่อเมนู กับ ราคา ไปใส่ใน lvOrderMenu แล้วบวกแต้มเพิ่ม และ บวกรวมเป็นเงินเพิ่ม
            if (btMenu3.Text != "Menu")
            {
                ListViewItem item = new ListViewItem(btMenu3.Text);
                item.SubItems.Add(menuPrice[2].ToString());
                lvOrderMenu.Items.Add(item);

                //บวกแต้มเพิ่ม ต้องตรวจสอบก่อนว่าเป็นสมาชิกไหม
                if (tbMemberName.Text != "(ชิ่อสมาชิก)")
                {
                    lbMemberScore.Text = (int.Parse(lbMemberScore.Text) + 1).ToString();
                }
                //บวกค่าเพิ่ม
                lbOrderPay.Text = (float.Parse(lbOrderPay.Text) + menuPrice[2]).ToString();
            }
        }

        private void btMenu4_Click(object sender, EventArgs e)
        {
            //ตรวจสอบว่าชื่อบนปุ่ม ต้องไม่ใช้คำว่า Menu (เพราะมันเป็น ดีฟอล ของ รูปที่ยังไม่ปรากฏ)
            //หากใช่ คำว่า Menu  ไม่ต้องทำอะไร   หากไม่ใช่คำว่า Menu แปลว่า มีรูปและชื่อ เมนูปรากฏ สามารถเลือกสั่งได้ 
            //หากไม่ใช่ ให้เอาชื่อเมนู กับ ราคา ไปใส่ใน lvOrderMenu แล้วบวกแต้มเพิ่ม และ บวกรวมเป็นเงินเพิ่ม
            if (btMenu4.Text != "Menu")
            {
                ListViewItem item = new ListViewItem(btMenu4.Text);
                item.SubItems.Add(menuPrice[3].ToString());
                lvOrderMenu.Items.Add(item);

                //บวกแต้มเพิ่ม ต้องตรวจสอบก่อนว่าเป็นสมาชิกไหม
                if (tbMemberName.Text != "(ชิ่อสมาชิก)")
                {
                    lbMemberScore.Text = (int.Parse(lbMemberScore.Text) + 1).ToString();
                }
                //บวกค่าเพิ่ม
                lbOrderPay.Text = (float.Parse(lbOrderPay.Text) + menuPrice[3]).ToString();
            }
        }

        private void btMenu5_Click(object sender, EventArgs e)
        {
            //ตรวจสอบว่าชื่อบนปุ่ม ต้องไม่ใช้คำว่า Menu (เพราะมันเป็น ดีฟอล ของ รูปที่ยังไม่ปรากฏ)
            //หากใช่ คำว่า Menu  ไม่ต้องทำอะไร   หากไม่ใช่คำว่า Menu แปลว่า มีรูปและชื่อ เมนูปรากฏ สามารถเลือกสั่งได้ 
            //หากไม่ใช่ ให้เอาชื่อเมนู กับ ราคา ไปใส่ใน lvOrderMenu แล้วบวกแต้มเพิ่ม และ บวกรวมเป็นเงินเพิ่ม
            if (btMenu5.Text != "Menu")
            {
                ListViewItem item = new ListViewItem(btMenu5.Text);
                item.SubItems.Add(menuPrice[4].ToString());
                lvOrderMenu.Items.Add(item);

                //บวกแต้มเพิ่ม ต้องตรวจสอบก่อนว่าเป็นสมาชิกไหม
                if (tbMemberName.Text != "(ชิ่อสมาชิก)")
                {
                    lbMemberScore.Text = (int.Parse(lbMemberScore.Text) + 1).ToString();
                }
                //บวกค่าเพิ่ม
                lbOrderPay.Text = (float.Parse(lbOrderPay.Text) + menuPrice[4]).ToString();
            }
        }

        private void btMenu6_Click(object sender, EventArgs e)
        {
            //ตรวจสอบว่าชื่อบนปุ่ม ต้องไม่ใช้คำว่า Menu (เพราะมันเป็น ดีฟอล ของ รูปที่ยังไม่ปรากฏ)
            //หากใช่ คำว่า Menu  ไม่ต้องทำอะไร   หากไม่ใช่คำว่า Menu แปลว่า มีรูปและชื่อ เมนูปรากฏ สามารถเลือกสั่งได้ 
            //หากไม่ใช่ ให้เอาชื่อเมนู กับ ราคา ไปใส่ใน lvOrderMenu แล้วบวกแต้มเพิ่ม และ บวกรวมเป็นเงินเพิ่ม
            if (btMenu6.Text != "Menu")
            {
                ListViewItem item = new ListViewItem(btMenu6.Text);
                item.SubItems.Add(menuPrice[5].ToString());
                lvOrderMenu.Items.Add(item);

                //บวกแต้มเพิ่ม ต้องตรวจสอบก่อนว่าเป็นสมาชิกไหม
                if (tbMemberName.Text != "(ชิ่อสมาชิก)")
                {
                    lbMemberScore.Text = (int.Parse(lbMemberScore.Text) + 1).ToString();
                }
                //บวกค่าเพิ่ม
                lbOrderPay.Text = (float.Parse(lbOrderPay.Text) + menuPrice[5]).ToString();
            }
        }

        private void btMenu7_Click(object sender, EventArgs e)
        {
            //ตรวจสอบว่าชื่อบนปุ่ม ต้องไม่ใช้คำว่า Menu (เพราะมันเป็น ดีฟอล ของ รูปที่ยังไม่ปรากฏ)
            //หากใช่ คำว่า Menu  ไม่ต้องทำอะไร   หากไม่ใช่คำว่า Menu แปลว่า มีรูปและชื่อ เมนูปรากฏ สามารถเลือกสั่งได้ 
            //หากไม่ใช่ ให้เอาชื่อเมนู กับ ราคา ไปใส่ใน lvOrderMenu แล้วบวกแต้มเพิ่ม และ บวกรวมเป็นเงินเพิ่ม
            if (btMenu7.Text != "Menu")
            {
                ListViewItem item = new ListViewItem(btMenu7.Text);
                item.SubItems.Add(menuPrice[6].ToString());
                lvOrderMenu.Items.Add(item);

                //บวกแต้มเพิ่ม ต้องตรวจสอบก่อนว่าเป็นสมาชิกไหม
                if (tbMemberName.Text != "(ชิ่อสมาชิก)")
                {
                    lbMemberScore.Text = (int.Parse(lbMemberScore.Text) + 1).ToString();
                }
                //บวกค่าเพิ่ม
                lbOrderPay.Text = (float.Parse(lbOrderPay.Text) + menuPrice[6]).ToString();
            }
        }

        private void btMenu8_Click(object sender, EventArgs e)
        {
            //ตรวจสอบว่าชื่อบนปุ่ม ต้องไม่ใช้คำว่า Menu (เพราะมันเป็น ดีฟอล ของ รูปที่ยังไม่ปรากฏ)
            //หากใช่ คำว่า Menu  ไม่ต้องทำอะไร   หากไม่ใช่คำว่า Menu แปลว่า มีรูปและชื่อ เมนูปรากฏ สามารถเลือกสั่งได้ 
            //หากไม่ใช่ ให้เอาชื่อเมนู กับ ราคา ไปใส่ใน lvOrderMenu แล้วบวกแต้มเพิ่ม และ บวกรวมเป็นเงินเพิ่ม
            if (btMenu8.Text != "Menu")
            {
                ListViewItem item = new ListViewItem(btMenu8.Text);
                item.SubItems.Add(menuPrice[7].ToString());
                lvOrderMenu.Items.Add(item);

                //บวกแต้มเพิ่ม ต้องตรวจสอบก่อนว่าเป็นสมาชิกไหม
                if (tbMemberName.Text != "(ชิ่อสมาชิก)")
                {
                    lbMemberScore.Text = (int.Parse(lbMemberScore.Text) + 1).ToString();
                }
                //บวกค่าเพิ่ม
                lbOrderPay.Text = (float.Parse(lbOrderPay.Text) + menuPrice[7]).ToString();
            }
        }

        private void btMenu9_Click(object sender, EventArgs e)
        {
            //ตรวจสอบว่าชื่อบนปุ่ม ต้องไม่ใช้คำว่า Menu (เพราะมันเป็น ดีฟอล ของ รูปที่ยังไม่ปรากฏ)
            //หากใช่ คำว่า Menu  ไม่ต้องทำอะไร   หากไม่ใช่คำว่า Menu แปลว่า มีรูปและชื่อ เมนูปรากฏ สามารถเลือกสั่งได้ 
            //หากไม่ใช่ ให้เอาชื่อเมนู กับ ราคา ไปใส่ใน lvOrderMenu แล้วบวกแต้มเพิ่ม และ บวกรวมเป็นเงินเพิ่ม
            if (btMenu9.Text != "Menu")
            {
                ListViewItem item = new ListViewItem(btMenu9.Text);
                item.SubItems.Add(menuPrice[8].ToString());
                lvOrderMenu.Items.Add(item);

                //บวกแต้มเพิ่ม ต้องตรวจสอบก่อนว่าเป็นสมาชิกไหม
                if (tbMemberName.Text != "(ชิ่อสมาชิก)")
                {
                    lbMemberScore.Text = (int.Parse(lbMemberScore.Text) + 1).ToString();
                }
                //บวกค่าเพิ่ม
                lbOrderPay.Text = (float.Parse(lbOrderPay.Text) + menuPrice[8]).ToString();
            }
        }

        private void btMenu10_Click(object sender, EventArgs e)
        {
            //ตรวจสอบว่าชื่อบนปุ่ม ต้องไม่ใช้คำว่า Menu (เพราะมันเป็น ดีฟอล ของ รูปที่ยังไม่ปรากฏ)
            //หากใช่ คำว่า Menu  ไม่ต้องทำอะไร   หากไม่ใช่คำว่า Menu แปลว่า มีรูปและชื่อ เมนูปรากฏ สามารถเลือกสั่งได้ 
            //หากไม่ใช่ ให้เอาชื่อเมนู กับ ราคา ไปใส่ใน lvOrderMenu แล้วบวกแต้มเพิ่ม และ บวกรวมเป็นเงินเพิ่ม
            if (btMenu10.Text != "Menu")
            {
                ListViewItem item = new ListViewItem(btMenu10.Text);
                item.SubItems.Add(menuPrice[9].ToString());
                lvOrderMenu.Items.Add(item);

                //บวกแต้มเพิ่ม ต้องตรวจสอบก่อนว่าเป็นสมาชิกไหม
                if (tbMemberName.Text != "(ชิ่อสมาชิก)")
                {
                    lbMemberScore.Text = (int.Parse(lbMemberScore.Text) + 1).ToString();
                }
                //บวกค่าเพิ่ม
                lbOrderPay.Text = (float.Parse(lbOrderPay.Text) + menuPrice[9]).ToString();
            }
        }

        private void btSave_Click(object sender, EventArgs e)
        {
            //ตรวจสอบก่อน ว่ารวมเป็นมีค่า 0.00 หรือเปล่า
            if (lbOrderPay.Text == "0.00")
            {
                MessageBox.Show("เลือกเมนูที่จะสั่งด้วย....!", "คำเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (rdMemberYes.Checked != true && rdMenberNo.Checked != true)
            {
                MessageBox.Show("เลือกสถานะการเป็นสมาชิก!", "คำเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (rdMemberYes.Checked == true && tbMemberName.Text == "(ชื่อสมาชิก)")
            {
                MessageBox.Show("กรุณาค้นหาสมาชิกด้วย...!", "คำเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                // ผ่านตรวจสอบมาได้ ต้องทำ 3 อย่าง
                // 1.บันทึกลง Order_tb (INSERT INTO......)
                // 2.บันทึกลง order_detail_tb  (INSERT INTO......)
                // 3.บันทึกแก้ไขแต้มคะแนนของสมาชิกที่ member_tb กรณีเป็นสมาชิก (UPDATE....SET...)

                using (SqlConnection sqlConnection = new SqlConnection(ShareResource.connectionString))
                {
                    try
                    {
                        sqlConnection.Open();

                        SqlTransaction sqlTransection = sqlConnection.BeginTransaction();//ใช้กับ Insert/update/delete
                        //บันทึกลง order_tb-----------
                        string strSQL1 = "INSERT INTO order_tb(memberId, orderPay, createAt, updateAt) " +
                                        "VALUE(@memberId, @orderPay, @createAt, @updateAt); " +
                                        "SELECT CAST(SCOPE_IDENTITY() AS INT)";

                        //ตัวแปรเก็บ OrderId
                        int orderId;

                        using (SqlCommand sqlCommand = new SqlCommand(strSQL1, sqlConnection, sqlTransection))
                        {
                            sqlCommand.Parameters.Add("@memberId", SqlDbType.Int).Value = memberId;
                            sqlCommand.Parameters.Add("@orderPay", SqlDbType.Float).Value = float.Parse(lbOrderPay.Text);
                            sqlCommand.Parameters.Add("@createAt", SqlDbType.Date).Value = DateTime.Now;
                            sqlCommand.Parameters.Add("@updateAt", SqlDbType.Date).Value = DateTime.Now;

                            orderId = (int)sqlCommand.ExecuteScalar();
                        }



                        //บันทึกลง order_detail_tb-----------
                        foreach (ListViewItem item in lvOrderMenu.Items)
                        {
                            string strSQL2 = "INSERT INTO order_detail_tb (orderId, menuName, menuPrice) " +
                                             "VALUE(@orderId, @menuName, @menuPrice) ";
                            using (SqlCommand sqlCommand = new SqlCommand(strSQL2, sqlConnection, sqlTransection))
                            {
                                //กำหนดให้กับSQL Parameter
                                sqlCommand.Parameters.Add("@orderId", SqlDbType.Int).Value = orderId;
                                sqlCommand.Parameters.Add("@menuName", SqlDbType.VarChar, 100).Value = item.SubItems[0].Text;
                                sqlCommand.Parameters.Add("@menuPrice", SqlDbType.Float).Value = float.Parse(item.SubItems[1].Text);


                                sqlCommand.ExecuteScalar();
                            }

                        }

                        //แก้ไข memberScore ที่ member_tb-----------
                        if (rdMemberYes.Checked == true)
                        {
                            string strSQL3 = "UPDATE member_tb SET memberScore=@memberScore WHERE member=@memberId";

                            using (SqlCommand sqlCommand = new SqlCommand(strSQL3, sqlConnection, sqlTransection))
                            {
                                //กำหนดให้กับSQL Parameter
                                sqlCommand.Parameters.Add("@memberScore", SqlDbType.Int).Value = int.Parse(lbMemberScore.Text);
                                sqlCommand.Parameters.Add("@memberId", SqlDbType.Int).Value = memberId;



                                sqlCommand.ExecuteScalar();
                            }



                        }

                        //----
                        sqlTransection.Commit();
                        MessageBox.Show("บันทึกเรียบร้อยแล้ว!");
                        resetForm();
                    }
                    catch (Exception ex)
                    {
                        //MessageBox.Show("พบข้อผิดพลาด กรุณาลองใหม่หรือติดต่อ IT : " + ex.Message);
                        MessageBox.Show("พบข้อผิดพลาด กรุณาลองใหม่หรือติดต่อ IT : " + ex.StackTrace);
                    }
                }
            }
        }
    }
}




