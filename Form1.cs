using System;
using System.Data;
using System.Windows.Forms;
using Microsoft.VisualBasic.ApplicationServices;
using Npgsql;

namespace GameStore
{
    public partial class Form1 : Form
    {
        private string connString = "Host=localhost;Port=5432;Username=postgres;Password=EarleS;Database=mteam"; // Veritabanı bağlantı dizesi
        public Form1()
        {
            InitializeComponent();
        }

        // DataGridView'yi veritabanındaki BaseUsers tablosu ile güncelleyen fonksiyon
        private void UpdateGrid()
        {

            // DataGridView'i temizle
            kullaniciGridTablo.DataSource = null;

            // Veritabanı bağlantısı kur
            using (NpgsqlConnection conn = new NpgsqlConnection(connString))
            {
                try
                {
                    conn.Open();

                    // Veritabanından BaseUsers tablosundaki tüm veriyi al
                    string query = @"
                                SELECT 
                                    b.user_id,
                                    b.username,
                                    b.email,
                                    b.registration_date,
                                    b.user_type,
                                    b.password,
                                    -- Admins için
                                    a.yetki_seviyesi,
                                    -- DevUsers için
                                    d.sirket_adi,
                                    d.onayli_gelistirici,
                                    -- NormalUsers için
                                    n.dogum_tarihi
                                FROM 
                                    baseusers b
                                LEFT JOIN admins a ON b.user_id = a.user_id
                                LEFT JOIN devusers d ON b.user_id = d.user_id
                                LEFT JOIN normalusers n ON b.user_id = n.user_id";
                    using (NpgsqlDataAdapter da = new NpgsqlDataAdapter(query, conn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        // DataGridView'e veriyi aktar
                        kullaniciGridTablo.DataSource = dt;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Veritabanı işlemi sırasında hata oluştu: " + ex.Message);
                }
            }
            dataGridView2.DataSource = null;

            // Veritabanı bağlantısı kur
            using (NpgsqlConnection conn = new NpgsqlConnection(connString))
            {
                try
                {
                    conn.Open();

                    // GameTags tablosundaki verileri sorgula
                    string query = "SELECT tag_id, tag_name FROM game_tags";

                    using (NpgsqlDataAdapter da = new NpgsqlDataAdapter(query, conn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        // Verileri DataGridView2'ye bağla
                        dataGridView2.DataSource = dt;


                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("GameTags yüklenirken bir hata oluştu: " + ex.Message);
                }
            }
            LoadPlatforms();
            LoadCategories();
            LoadPublishers();
            // DataGridView'i sıfırla
            dataGridView1.DataSource = null;

            // Veritabanı bağlantısını kur
            using (NpgsqlConnection conn = new NpgsqlConnection(connString))
            {
                try
                {
                    conn.Open();

                    // Veritabanından oyun bilgilerini almak için sorgu
                    string query = @"
                    SELECT 
                        g.game_id AS ""Oyun ID"",
                        g.game_name AS ""Oyun Adı"",
                        g.price AS ""Fiyat"",
                        p.publisher_name AS ""Yayıncı"",
                        bu.username AS ""Yapımcı"",
                        c.category_name AS ""Kategori""
                    FROM 
                        games g
                    LEFT JOIN publishers p ON g.publisher_id = p.publisher_id
                    LEFT JOIN devusers d ON g.developer_id = d.user_id
                    LEFT JOIN baseusers bu ON d.user_id = bu.user_id
                    LEFT JOIN categories c ON g.category_id = c.category_id";

                    // Verileri doldurmak için DataAdapter kullan
                    using (NpgsqlDataAdapter da = new NpgsqlDataAdapter(query, conn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        // DataGridView'e veriyi aktar
                        dataGridView1.DataSource = dt;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Veritabanı işlemi sırasında hata oluştu: " + ex.Message);
                }
            }

        }


        private void LoadPlatforms()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT platform_id, platform_name FROM platforms";
                    using (NpgsqlDataAdapter da = new NpgsqlDataAdapter(query, conn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dataGridView3.DataSource = dt;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Platforms yüklenirken hata oluştu: " + ex.Message);
                }
            }
        }
        private void LoadCategories()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT category_id, category_name FROM categories";
                    using (NpgsqlDataAdapter da = new NpgsqlDataAdapter(query, conn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dataGridView4.DataSource = dt; 
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Categories yüklenirken hata oluştu: " + ex.Message);
                }
            }
        }
        private void LoadPublishers()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT publisher_id, publisher_name FROM publishers";
                    using (NpgsqlDataAdapter da = new NpgsqlDataAdapter(query, conn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dataGridView5.DataSource = dt; 
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Publishers yüklenirken hata oluştu: " + ex.Message);
                }
            }
        }
        private void SearchUser(string username)
        {
            // DataGridView'i temizle
            kullaniciGridTablo.DataSource = null;

            // Veritabanı bağlantısı kur
            using (NpgsqlConnection conn = new NpgsqlConnection(connString))
            {
                try
                {
                    conn.Open();

                    // Girilen kullanıcı adına göre veri sorgula
                    string query = @"
                SELECT 
                    b.user_id,
                    b.username,
                    b.email,
                    b.registration_date,
                    b.user_type,
                    b.password,
                    -- Admins için
                    a.yetki_seviyesi,
                    -- DevUsers için
                    d.sirket_adi,
                    d.onayli_gelistirici,
                    -- NormalUsers için
                    n.dogum_tarihi
                FROM 
                    baseusers b
                LEFT JOIN admins a ON b.user_id = a.user_id
                LEFT JOIN devusers d ON b.user_id = d.user_id
                LEFT JOIN normalusers n ON b.user_id = n.user_id
                WHERE LOWER(b.username) = LOWER(@username)";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        // Kullanıcı adı parametresini ekle
                        cmd.Parameters.AddWithValue("@username", username);

                        using (var da = new NpgsqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);

                            // Eğer sonuç yoksa uyarı ver
                            if (dt.Rows.Count == 0)
                            {
                                MessageBox.Show("Aradığınız kullanıcı bulunamadı!");
                            }

                            // Sonuçları DataGridView'e aktar
                            kullaniciGridTablo.DataSource = dt;

                            int userId = Convert.ToInt32(dt.Rows[0]["user_id"]);

                            ListUserGames(userId);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Veritabanı işlemi sırasında hata oluştu: " + ex.Message);
                }
            }
        }

        private void ListUserGames(int userId)
        {

            dataGridView7.DataSource = null;

            // Veritabanı bağlantısı kur
            using (NpgsqlConnection conn = new NpgsqlConnection(connString))
            {
                try
                {
                    conn.Open();

                    // Kullanıcıya ait favori oyunları sorgula
                    string query = @"
                SELECT 
                    g.game_name, 
                    g.price
                FROM 
                    favorite_games fg
                JOIN 
                    games g ON fg.game_id = g.game_id
                WHERE 
                    fg.user_id = @userId";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        // Kullanıcı ID'sini parametre olarak ekle
                        cmd.Parameters.AddWithValue("@userId", userId);

                        using (var da = new NpgsqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);

                            // Favori oyunları DataGridView'e aktar
                            dataGridView7.DataSource = dt;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Veritabanı işlemi sırasında hata oluştu: " + ex.Message);
                }
            }
        }
        private void AddGame(int categoryId, int developerId, string gameName, decimal price, int publisherId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT add_game(@category_id, @developer_id, @game_name, @price, @publisher_id)";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@category_id", categoryId);
                        cmd.Parameters.AddWithValue("@developer_id", developerId);
                        cmd.Parameters.AddWithValue("@game_name", gameName);
                        cmd.Parameters.AddWithValue("@price", price);
                        cmd.Parameters.AddWithValue("@publisher_id", publisherId);

                        cmd.ExecuteNonQuery(); // Fonksiyonu çalıştır
                        MessageBox.Show("Oyun başarıyla eklendi!");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Oyun eklenirken bir hata oluştu: " + ex.Message);
                }
            }
        }





        private void Form1_Load(object sender, EventArgs e)
        {
            UpdateGrid(); // Uygulama açıldığında veritabanını yükle
        }

        // Button1'e tıklandığında, veri ekleme işlemi sonrasında gridin güncellenmesi
        private void button1_Click(object sender, EventArgs e)
        {
            string username = textBox1.Text;
            string email = textBox2.Text;
            DateTime dogumTarihi = dateTimePicker1.Value;

            string insertBaseUserQuery = "INSERT INTO baseusers (username, email, user_type) VALUES (@username, @email, 'Normal') RETURNING user_id";

            using (NpgsqlConnection conn = new NpgsqlConnection(connString))
            {
                try
                {
                    conn.Open();

                    // BaseUsers tablosuna veri ekle
                    using (NpgsqlCommand cmd = new NpgsqlCommand(insertBaseUserQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@email", email);

                        // user_id'yi al
                        int userId = Convert.ToInt32(cmd.ExecuteScalar());

                        // NormalUsers tablosuna veri ekle
                        string insertNormalUserQuery = "INSERT INTO normalusers (user_id, dogum_tarihi) VALUES (@user_id, @dogum_tarihi)";
                        using (NpgsqlCommand cmd2 = new NpgsqlCommand(insertNormalUserQuery, conn))
                        {
                            cmd2.Parameters.AddWithValue("@user_id", userId);
                            cmd2.Parameters.AddWithValue("@dogum_tarihi", dogumTarihi);

                            cmd2.ExecuteNonQuery(); // NormalUsers tablosuna veri ekle
                        }

                        MessageBox.Show("Kullanıcı başarıyla eklendi.");

                        // İşlem bittiğinde grid'i güncelle
                        UpdateGrid();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Veritabanı işlemi sırasında hata oluştu: " + ex.Message);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

            int userIdToDelete = Convert.ToInt32(textBox3.Text);


            using (var conn = new NpgsqlConnection(connString))
            {
                try
                {
                    conn.Open();

                    // Transaction başlat
                    using (var transaction = conn.BeginTransaction())
                    {
                        // SQL fonksiyonunu çağır
                        string query = "SELECT delete_user_by_id(@user_id)";
                        using (var cmd = new NpgsqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@user_id", userIdToDelete);
                            cmd.ExecuteNonQuery();
                        }

                        // Transaction'ı commit et
                        transaction.Commit();
                    }

                    // Veritabanındaki veriyi yeniden yükle ve grid'i güncelle
                    UpdateGrid();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message);
                }
            }
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back && e.KeyChar != (char)Keys.Enter)
            {
                e.Handled = true; // Geçersiz tuşu engelle
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            string username = textBox4.Text;
            string email = textBox5.Text;
            string companyName = textBox6.Text;
            bool isVerified = checkBox1.Checked;

            // Veritabanı bağlantı dizesi

            // Bağlantıyı kullanarak işlemi gerçekleştirme
            using (var conn = new NpgsqlConnection(connString))
            {
                try
                {
                    // Veritabanı bağlantısını aç
                    conn.Open();

                    using (var transaction = conn.BeginTransaction())
                    {
                        // SQL fonksiyonunu çağır
                        string query = "SELECT add_developer(@username, @email, @companyName, @isVerified)";
                        using (var cmd = new NpgsqlCommand(query, conn))
                        {
                            // Parametreleri ekle
                            cmd.Parameters.AddWithValue("username", username);
                            cmd.Parameters.AddWithValue("email", email);
                            cmd.Parameters.AddWithValue("companyName", companyName);
                            cmd.Parameters.AddWithValue("isVerified", isVerified);

                            // Fonksiyonu çalıştır
                            cmd.ExecuteNonQuery();
                        }

                        // Transaction'ı commit et
                        transaction.Commit();
                    }

                    // Veritabanındaki veriyi yeniden yükle ve grid'i güncelle
                    UpdateGrid();
                    MessageBox.Show("Developer added successfully!");
                }
                catch (Exception ex)
                {
                    // Hata durumunda mesaj göster
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            string username = textBox7.Text;
            string email = textBox8.Text;
            int authorityLevel;

            // TextBox9'daki değeri integer olarak al
            if (!int.TryParse(textBox9.Text, out authorityLevel))
            {
                MessageBox.Show("Yetki seviyesi geçerli bir sayı olmalıdır!");
                return;
            }

            // Veritabanı bağlantı dizesi
            string connString = "Host=localhost;Port=5432;Username=postgres;Password=EarleS;Database=mteam";

            using (var conn = new NpgsqlConnection(connString))
            {
                try
                {
                    conn.Open();

                    using (var transaction = conn.BeginTransaction())
                    {
                        // SQL fonksiyonunu çağır
                        string query = "SELECT add_admin(@username, @email, @authorityLevel)";
                        using (var cmd = new NpgsqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("username", username);
                            cmd.Parameters.AddWithValue("email", email);
                            cmd.Parameters.AddWithValue("authorityLevel", authorityLevel);

                            cmd.ExecuteNonQuery();
                        }

                        // Transaction'ı commit et
                        transaction.Commit();
                    }

                    // Veritabanındaki veriyi yeniden yükle ve grid'i güncelle
                    UpdateGrid();
                    MessageBox.Show("Admin added successfully!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string username = textBox10.Text.Trim();

            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Lütfen aramak istediğiniz kullanıcı adını girin!");
                return;
            }

            // Kullanıcıyı ara
            SearchUser(username);
        }
        private void button6_Click(object sender, EventArgs e)
        {
            // TextBox'lardan değerleri al
            int userIdToUpdate;
            int newAuthorityLevel;

            if (!int.TryParse(textBox11.Text, out userIdToUpdate))
            {
                MessageBox.Show("Lütfen geçerli bir kullanıcı ID girin!");
                return;
            }

            if (!int.TryParse(textBox12.Text, out newAuthorityLevel))
            {
                MessageBox.Show("Lütfen geçerli bir yetki seviyesi girin!");
                return;
            }

            using (var conn = new NpgsqlConnection(connString))
            {
                try
                {
                    conn.Open();

                    // SQL fonksiyonunu çağır
                    string query = "SELECT update_admin_authority(@user_id_to_update, @new_authority_level)";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@user_id_to_update", userIdToUpdate);
                        cmd.Parameters.AddWithValue("@new_authority_level", newAuthorityLevel);

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Yetki seviyesi başarıyla güncellendi!");

                    // Güncellemeden sonra grid'i yenile
                    UpdateGrid();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message);
                }
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                // TextBox'lardan verileri al
                int categoryId = int.Parse(textBox16.Text);
                int developerId = int.Parse(textBox14.Text);
                string gameName = textBox13.Text;
                decimal price = decimal.Parse(textBox17.Text);
                int publisherId = int.Parse(textBox15.Text);

                // Fonksiyonu çağır
                AddGame(categoryId, developerId, gameName, price, publisherId);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Girdi hatası: " + ex.Message);
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            // TextBox'lardan game_id ve tag_id alınır
            int gameId;
            int tagId;

            // Inputların geçerli olduğundan emin olun
            if (int.TryParse(textBox18.Text, out gameId) && int.TryParse(textBox19.Text, out tagId))
            {
                // SQL sorgusu hazırlanır
                string insertQuery = "INSERT INTO Game_GameTags (game_id, tag_id) VALUES (@game_id, @tag_id)";

                using (NpgsqlConnection conn = new NpgsqlConnection(connString))
                {
                    try
                    {
                        conn.Open();

                        // SQL komutu hazırlanır
                        using (NpgsqlCommand cmd = new NpgsqlCommand(insertQuery, conn))
                        {
                            // Parametreler eklenir
                            cmd.Parameters.AddWithValue("@game_id", gameId);
                            cmd.Parameters.AddWithValue("@tag_id", tagId);

                            // Komut çalıştırılır
                            cmd.ExecuteNonQuery();

                            // Başarı mesajı gösterilir
                            MessageBox.Show("Oyun etiketlendi.");


                            UpdateGrid();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Hata oluştu: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Lütfen geçerli bir game_id ve tag_id giriniz.");
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            // TextBox'lardan game_id ve platform_id alınır
            int gameId;
            int platformId;


            if (int.TryParse(textBox18.Text, out gameId) && int.TryParse(textBox19.Text, out platformId))
            {
                // SQL sorgusu hazırlanır
                string insertQuery = "INSERT INTO Game_Platforms (game_id, platform_id) VALUES (@game_id, @platform_id)";

                using (NpgsqlConnection conn = new NpgsqlConnection(connString))
                {
                    try
                    {
                        conn.Open();

                        // SQL komutu hazırlanır
                        using (NpgsqlCommand cmd = new NpgsqlCommand(insertQuery, conn))
                        {
                            // Parametreler eklenir
                            cmd.Parameters.AddWithValue("@game_id", gameId);
                            cmd.Parameters.AddWithValue("@platform_id", platformId);

                            // Komut çalıştırılır
                            cmd.ExecuteNonQuery();

                            MessageBox.Show("Oyun platforma bağlandı.");

                            // Gerekirse UI güncellenebilir (örneğin bir grid kontrolü)
                            UpdateGrid();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Hata oluştu: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Lütfen geçerli bir game_id ve platform_id giriniz.");
            }
        }

        private void button20_Click(object sender, EventArgs e)
        {
            int userId;
            int gameId;

            try
            {
                // TextBox'lardan User ID ve Game ID'yi al
                userId = int.Parse(textBox20.Text); 
                gameId = int.Parse(textBox21.Text); 

                using (NpgsqlConnection conn = new NpgsqlConnection(connString))
                {
                    conn.Open();

                    // Satın alımı eklemek için SQL sorgusu
                    string query = @"
                INSERT INTO Purchases (user_id, game_id, purchase_date)
                VALUES (@userId, @gameId, CURRENT_TIMESTAMP)";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        // Parametreleri ayarla
                        cmd.Parameters.AddWithValue("@userId", userId);
                        cmd.Parameters.AddWithValue("@gameId", gameId);

                        // Sorguyu çalıştır
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Satın alma başarıyla eklendi!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Satın alma eklenirken bir sorun oluştu.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Lütfen geçerli bir User ID ve Game ID giriniz!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
