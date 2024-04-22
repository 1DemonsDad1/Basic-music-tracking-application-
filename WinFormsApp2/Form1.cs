using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace WinFormsApp2
{
    public partial class Form1 : Form
    {
        private List<Song> songsList = new List<Song>();

        public Form1()
        {
            InitializeComponent();

            // Add these lines to set up columns programmatically
            dataGridView1.Columns.Add("Author", "Izvajalec pesmi");
            dataGridView1.Columns.Add("SongName", "Naslov pesmi");
            dataGridView1.Columns.Add("Comment", "Komentar");

            // Load data when the form is loaded
            LoadDataFromFile();

            // Attach the FormClosing event handler
            this.FormClosing += Form1_FormClosing;

            // Attach the TextChanged event handler to the search box
            textBox1.TextChanged += textBox1_TextChanged;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Prompt for Author
            string author = PromptUser("Vnesite izvajalca pesmi:");

            // If the user cancels, return
            if (author == null)
                return;

            // Prompt for Song Name
            string songName = PromptUser("Vnesite naslov pesmi:");

            // If the user cancels, return
            if (songName == null)
                return;

            // Prompt for Comment
            string comment = PromptUser("Vnesite opombo(neobvezno):");

            // If the user cancels, return
            if (comment == null)
                return;

            // Add a new song to the list
            Song newSong = new Song(author, songName, comment);
            songsList.Add(newSong);

            // Update the DataGridView
            UpdateDataGridView();
        }

        // Function to prompt the user with a custom message and get input
        private string PromptUser(string message)
        {
            string userInput = Microsoft.VisualBasic.Interaction.InputBox(message, "Input", "");

            // If the user clicks Cancel, return null
            if (string.IsNullOrWhiteSpace(userInput))
                return null;

            return userInput;
        }

        // Update the DataGridView with the songsList
        private void UpdateDataGridView(List<Song> songs)
        {
            dataGridView1.Rows.Clear();

            foreach (var song in songs)
            {
                dataGridView1.Rows.Add(song.Author, song.SongName, song.Comment);
            }
        }

        // Overloaded UpdateDataGridView method to accept no arguments
        private void UpdateDataGridView()
        {
            UpdateDataGridView(songsList);
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Your code here or leave it empty
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Check if any rows are selected
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Create a list to store the indices of selected rows
                List<int> selectedIndices = new List<int>();

                // Iterate through all selected rows to store indices
                foreach (DataGridViewRow selectedRow in dataGridView1.SelectedRows)
                {
                    selectedIndices.Add(selectedRow.Index);
                }

                // Sort the indices in descending order
                selectedIndices.Sort((a, b) => b.CompareTo(a));

                // Iterate through sorted indices and remove songs from the list
                foreach (int selectedIndex in selectedIndices)
                {
                    if (selectedIndex >= 0 && selectedIndex < songsList.Count)
                    {
                        songsList.RemoveAt(selectedIndex);
                    }
                }

                // Update the DataGridView after removing selected rows
                UpdateDataGridView();
            }
            else
            {
                MessageBox.Show("Izberite eno ali veè vrstic za brisanje.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Check if any rows are selected
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Prompt user for file path
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
                saveFileDialog.Title = "Save Selected Rows";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Get the selected file path
                    string filePath = saveFileDialog.FileName;

                    try
                    {
                        // Create a new StreamWriter and write the selected rows to the file
                        using (System.IO.StreamWriter sw = new System.IO.StreamWriter(filePath))
                        {
                            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                            {
                                // Construct a tab-separated string of the row's cell values
                                string rowData = $"{row.Cells["Author"].Value}\t{row.Cells["SongName"].Value}\t{row.Cells["Comment"].Value}";

                                // Write the row data to the file
                                sw.WriteLine(rowData);
                            }
                        }

                        MessageBox.Show("Izbrane vrstice so bile izvožene v besedilno datoteko.", "Export Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred while exporting the selected rows: {ex.Message}", "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Izberite eno ali veè vrstic za izvoz.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Save data when the form is closing
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveDataToFile();
        }

        private void SaveDataToFile()
        {
            try
            {
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter("data.txt"))
                {
                    foreach (var song in songsList)
                    {
                        // Write each song data to a line in the file
                        sw.WriteLine($"{song.Author}\t{song.SongName}\t{song.Comment}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while saving data: {ex.Message}", "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadDataFromFile()
        {
            try
            {
                if (System.IO.File.Exists("data.txt"))
                {
                    // Read each line from the file and create a Song object
                    foreach (string line in System.IO.File.ReadLines("data.txt"))
                    {
                        string[] parts = line.Split('\t');
                        if (parts.Length == 3)
                        {
                            Song loadedSong = new Song(parts[0], parts[1], parts[2]);
                            songsList.Add(loadedSong);
                        }
                    }

                    // Update the DataGridView after loading data
                    UpdateDataGridView();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading data: {ex.Message}", "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            // Call the search method when the text in the search box changes
            Search(textBox1.Text);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // Call the search method when the search button is clicked
            Search(textBox1.Text);
        }

        // Search method to filter rows based on the search term
        private void Search(string searchTerm)
        {
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                // Filter songsList based on the search term
                List<Song> searchResults = songsList.Where(song =>
                    song.Author.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    song.SongName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    song.Comment.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                // Update the DataGridView with the search results
                UpdateDataGridView(searchResults);
            }
            else
            {
                // If the search term is empty, show all rows
                UpdateDataGridView();
            }
        }

        // Class to represent a song
        public class Song
        {
            public string Author { get; set; }
            public string SongName { get; set; }
            public string Comment { get; set; }

            public Song(string author, string songName, string comment)
            {
                Author = author;
                SongName = songName;
                Comment = comment;
            }
        }
    }
}
