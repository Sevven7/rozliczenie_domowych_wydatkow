using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace DPM_v._0._1
{
    public partial class Form1 : Form
    {
        private static readonly string connectionString = "Data Source=C:\\Users\\micha\\Desktop\\DPM\\BazaSQLite";
        private string wartosc1 = string.Empty;
        private string wartosc2 = string.Empty;
        private string wartoscWyplata = string.Empty;

        public Form1()
        {
            InitializeComponent();
            WczytajDane();
            ObliczSume();
            ObliczRoznice();
            AktualizujProgres();
            textBox1.KeyPress += TextBox1_KeyPress;
            textBox2.KeyPress += TextBox2_KeyPress;
            textBox4.TextChanged += textBox4_TextChanged;
            listBox1.SelectedIndexChanged += listBox1_SelectedIndexChanged;
            textBox5.TextChanged += textBox5_TextChanged;
            this.Load += Form1_Load;
        }

        private void TextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            System.Windows.Forms.TextBox textBox = (System.Windows.Forms.TextBox)sender;
            if (e.KeyChar == '.' && textBox.Text.Contains('.'))
            {
                e.Handled = true;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox2.KeyPress += TextBox2_KeyPress;
            textBox1.KeyPress += TextBox1_KeyPress;
            textBox4.TextChanged += textBox4_TextChanged;
        }

        private void TextBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                if (textBox1.Text.Length > 0)
                {
                    listBox1.Items.Add(textBox1.Text);
                    listBox2.Items.Add(textBox2.Text);
                    AktualizujProgres();
                    textBox1.Text = "";
                    textBox2.Text = "";
                }
                else
                {
                    //MessageBox.Show("Wartość jest pusta", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            wartosc1 = listBox1.Text;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            wartosc2 = listBox2.Text;
        }

        private void WczytajDane()
        {
            SQLiteConnection connection = new SQLiteConnection(connectionString);

            try
            {
                connection.Open();

                using (SQLiteCommand selectCommand = new SQLiteCommand("SELECT * FROM MojaTabela;", connection))
                {
                    using (SQLiteDataReader reader = selectCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listBox1.Items.Add(reader["Kolumna1"].ToString());
                            listBox2.Items.Add(reader["Kolumna2"].ToString());
                        }
                    }
                }

                // Wczytaj dane z MojaTabela2
                using (SQLiteCommand selectCommand = new SQLiteCommand("SELECT * FROM MojaTabela2;", connection))
                {
                    using (SQLiteDataReader reader = selectCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            wartoscWyplata = reader["Wplata"].ToString();
                            textBox4.Text = wartoscWyplata;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas wczytywania danych: {ex.Message}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                connection.Close();
            }
        }

        private void ZapiszWplateDoBazy()
        {

            SQLiteConnection connection = new SQLiteConnection(connectionString);

            try
            {
                connection.Open();

                using (SQLiteCommand deleteCommand = new SQLiteCommand("DELETE FROM MojaTabela2;", connection))
                {
                    deleteCommand.ExecuteNonQuery();
                }

                using (SQLiteCommand updateCommand = new SQLiteCommand("INSERT INTO MojaTabela2 (Wplata) VALUES (@WartoscWyplata);", connection))
                {
                    updateCommand.Parameters.AddWithValue("@WartoscWyplata", wartoscWyplata);
                    updateCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas zapisywania danych: {ex.Message}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                connection.Close();
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            ZapiszWplateDoBazy();
            SQLiteConnection connection = new SQLiteConnection(connectionString);

            try
            {
                connection.Open();

                using (SQLiteCommand deleteCommand = new SQLiteCommand("DELETE FROM MojaTabela;", connection))
                {
                    deleteCommand.ExecuteNonQuery();
                }

                for (int i = 0; i < listBox1.Items.Count; i++)
                {
                    using (SQLiteCommand insertCommand = new SQLiteCommand("INSERT INTO MojaTabela (Kolumna1, Kolumna2) VALUES (@Wartosc1, @Wartosc2);", connection))
                    {
                        insertCommand.Parameters.AddWithValue("@Wartosc1", listBox1.Items[i].ToString());
                        insertCommand.Parameters.AddWithValue("@Wartosc2", listBox2.Items[i].ToString());

                        insertCommand.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Dane zapisane do bazy.", "Sukces", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas zapisywania danych: {ex.Message}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                connection.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (progressBar1.Value < 100)
            {
                if (textBox1.Text.Length > 0)
                {
                    if (listBox1.Items.Contains(textBox1.Text))
                    {
                        DialogResult result = MessageBox.Show("Podobny element o takiej samej nazwie lub cenie już istnieje! Czy chcesz go dodać?", "Uwaga!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (result == DialogResult.No)
                        {
                            return;
                        }
                    }
                    listBox1.Items.Add(textBox1.Text);
                    listBox2.Items.Add(textBox2.Text);
                    AktualizujProgres();
                    textBox1.Text = "";
                    textBox2.Text = "";
                    ObliczSume();
                    ObliczRoznice();
                }
                else
                {
                    MessageBox.Show("Wartość jest pusta", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Lista jest już pełna", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ObliczSume();
            wartosc1 = listBox1.SelectedItem?.ToString();
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            wartosc2 = listBox2.SelectedItem?.ToString();
        }

        private void button3_Click(object sender, EventArgs e)
        {

            int i = listBox1.SelectedIndex;
            int j = listBox2.SelectedIndex;
            if (i != -1 && j != -1)
            {
                listBox1.Items.RemoveAt(i);
                listBox2.Items.RemoveAt(j);
                AktualizujProgres();
                ObliczSume();
                ObliczRoznice();
            }
            else
            {
                MessageBox.Show("Zaznacz poprawnie cenę oraz produkt który chcesz usunąć z listy!", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void progressBar1_Click(object sender, EventArgs e) { }

        private void AktualizujProgres()
        {
            int i = listBox1.Items.Count;
            progressBar1.Value = i;
        }

        private void label4_Click(object sender, EventArgs e) { }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            wartoscWyplata = textBox4.Text;

        }

        private void textBox4_TextChanged_1(object sender, EventArgs e) //środki które zaoszczędzisz
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e) //środki zaoszczędzone w zeszłym miesiącu
        {
            ObliczSume();
        }

        private void ObliczSume()
        {
            // Zainicjuj zmienną na sumę
            int suma = 0;

            // Iteruj przez każdy element w listBox1, konwertuj na liczbę i dodaj do sumy
            foreach (string element in listBox1.Items)
            {
                Console.WriteLine($"Przetwarzam: {element}");
                if (int.TryParse(element.Trim(), out int liczba))
                {
                    suma += liczba;
                }
                else
                {
                    MessageBox.Show($"Nie można przekonwertować '{element}' na liczbę.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; // Przerwij obliczenia w przypadku błędu
                }
            }

            // Ustaw wartość textBox5 na wyliczoną sumę
            textBox5.Text = suma.ToString();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            ObliczRoznice();
        }

        private void ObliczRoznice()
        {
            if (int.TryParse(textBox4.Text, out int wartosc4) && int.TryParse(textBox5.Text, out int wartosc5))
            {
                int roznica = wartosc4 - wartosc5;
                textBox3.Text = roznica.ToString();
            }
            else
            {
                MessageBox.Show("Nie można przekonwertować wartości na liczby całkowite.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void listBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
/*  private void button3_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            AktualizujProgres();
        }
*/

