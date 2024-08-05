using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace FITUR
{
    public partial class Form1 : Form
    {        
        public Form1()
        {
            InitializeComponent();
        }
        private string uploadedFilePath  = string.Empty;
       
        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog openfile = new OpenFileDialog();
            // Set filter untuk file C dan C++
            openfile.Filter = "C# Files (*.cs)|*.cs|Java Files (*.java)|*.java|C++ Files ( *.cpp)|*.cpp|PHP Files (*.php)|*.php";

            DialogResult result = openfile.ShowDialog();

            if (result == DialogResult.OK)
            {
                uploadedFilePath = openfile.FileName;
                MMetric_UpldBox.Text = openfile.FileName;
            }
        }
        private void A_btn_reset_Click(object sender, EventArgs e)
        {
            uploadedFilePath = string.Empty;
            txtOutput1.Clear();
            MMetric_UpldBox.Clear();
            
        }
        //START line of code (LOC)

        private void A_btn_run_Click(object sender, EventArgs e)
        {
            txtOutput1.Clear();

            txtOutput3.Clear();

            if (!string.IsNullOrEmpty(uploadedFilePath) && File.Exists(uploadedFilePath))
            {
                string inicodingnya = File.ReadAllText(uploadedFilePath);
                try
                {
                    //string[] kode = inicodingnya.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                    string[] kode = inicodingnya.Split('\n');

                    int jumlahBarisKode = kode.Length;
                    int jumlahBarisKosong = 0;
                    int jumlahBarisKomentar = 0;
                    bool multiLineComment = false; // untuk menangani komentar multi-baris
                    foreach (string line in kode)
                    {
                        if (string.IsNullOrWhiteSpace(line))
                        {
                            // Baris kosong
                            jumlahBarisKosong++;
                        }
                        else if (line.Trim().StartsWith("//"))
                        {
                            // Baris komentar satu baris
                            jumlahBarisKomentar++;

                        }
                        else if (line.Trim().StartsWith("/*"))
                        {
                            // Baris komentar multi-baris dimulai
                            multiLineComment = true;
                            jumlahBarisKomentar++;
                        }
                        else if (line.Trim().EndsWith("*/"))
                        {
                            // Baris komentar multi-baris berakhir
                            jumlahBarisKomentar++;
                            multiLineComment = false;
                        }
                        else if (multiLineComment)
                        {
                            // Baris dalam komentar multi-baris
                            jumlahBarisKomentar++;
                        }                                                                                                              
                    }
                    string hasil = "Jumlah baris kode: " + jumlahBarisKode + " baris" + Environment.NewLine +
                                    "Jumlah baris kosong: " + jumlahBarisKosong + " baris" + Environment.NewLine +
                                    "Jumlah baris komentar: " + jumlahBarisKomentar + " baris";

                    txtOutput1.Text = hasil;
                }
                catch (FileNotFoundException ex)
                {
                    MessageBox.Show($"File not found: {ex.FileName}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Select a file first.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return;
        }
        //END OF LOC

        //Start cyclomatic 
        private void btnHitung_Click(object sender, EventArgs e)
        {
            txtOutput1.Clear();
            
            txtOutput3.Clear();

            if (!string.IsNullOrEmpty(uploadedFilePath) && File.Exists(uploadedFilePath))
            {
                KompleksitasSiklomata(uploadedFilePath);

                //string code = File.ReadAllText((uploadedFilePath));
                //Menampilkan kode yang diupload
                
                
                //txtOutput2.Text = uploadedFilePath;


            }
            else
            {
                MessageBox.Show("Select a file first.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            //return ;
        }

        private void KompleksitasSiklomata(string filePath)
        {
            if (File.Exists(filePath))
            {
                string code = File.ReadAllText(filePath);

                // Hitung jumlah tepi
                int jumlahTepi = HitungEdge(code);

                // Hitung jumlah simpul
                int jumlahSimpul = HitungNode(code);

                // Hitung jumlah Komponen
                int jumlahKomponen = HitungKomponenTerhubung(code);

                // Hitung metrik tambahan sesuai kebutuhan (contoh: panjang rata-rata baris kode)
                double komplesitasSiklomata = jumlahTepi - jumlahSimpul + 2 * jumlahKomponen;

                // Menampilkan metrik di TextBox
                string metrikMessage = //$"FileName: {Path.GetFileName(filePath)}\r\n\n" +
                                       $"Total Number of Edges: {jumlahTepi}\r\n" +
                                       $"Total Number of Nodes: {jumlahSimpul}\r\n" +
                                       $"Total Number of Connected Components: {jumlahKomponen}\r\n";
                                       //$"Cyclomatic Complexity (V(G) = E - N + 2P) = {komplesitasSiklomata}\r\n";

                txtOutput1.Text = metrikMessage;
                //uploadedFilePath = code;
                
                txtOutput3.Text = $"Cyclomatic Complexity (V(G) = E - N + 2P)\r\n" +
                                  $"Cyclomatic Complexity (V(G) = {jumlahTepi} - {jumlahSimpul} + 2*({jumlahKomponen}))\r\n" +
                                  $"Cyclomatic Complexity (V(G) = {komplesitasSiklomata}\r\n";


                //txtOutput2.Text = code;
            }
            else
            {
                MessageBox.Show("File not found.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private static int HitungEdge(string code)
        {
            // Implementasi kompleksitas siklomatik di sini (gunakan metode sebelumnya atau metode lainnya)
            int edgeCount = 0;
            string[] lines = code.Split('\n');

            bool isInIfBlock = false;
            bool isInSwitchBlock = false;

            foreach (string line in lines)
            {
                string trimmedLine = line.Trim();

                if (trimmedLine.StartsWith("if") || trimmedLine.StartsWith("else") || trimmedLine.StartsWith("else if") || trimmedLine.StartsWith("while") ||
                    trimmedLine.StartsWith("for") || trimmedLine.StartsWith("case") || trimmedLine.StartsWith("catch") ||
                    trimmedLine.StartsWith("switch") || trimmedLine.StartsWith("try") || trimmedLine.StartsWith("&&") || trimmedLine.StartsWith("||"))
                {
                    edgeCount++;

                    // Set isInIfBlock ke true ketika memasuki blok "if"
                    if (trimmedLine.StartsWith("if"))
                    {
                        isInIfBlock = true;
                    }
                    // Tambahkan tepi tambahan untuk setiap "else" setelah "if"
                    else if (trimmedLine.StartsWith("else") && isInIfBlock)
                    {
                        edgeCount++;
                    }

                    // Set isInSwitchBlock ke true ketika memasuki blok "switch"
                    if (trimmedLine.StartsWith("switch"))
                    {
                        isInSwitchBlock = true;
                    }
                    // Tambahkan tepi tambahan untuk setiap "case" dalam "switch"
                    else if (trimmedLine.StartsWith("case") && isInSwitchBlock)
                    {
                        edgeCount++;
                    }
                }

                // Set isInIfBlock ke false ketika keluar dari blok "if"
                if (isInIfBlock && trimmedLine.EndsWith("}"))
                {
                    isInIfBlock = false;
                }

                // Set isInSwitchBlock ke false ketika keluar dari blok "switch"
                if (isInSwitchBlock && trimmedLine.EndsWith("}"))
                {
                    isInSwitchBlock = false;
                }
            }
            return edgeCount + 1;
        }

        private int HitungNode(string code)
        {
            // Implementasi kompleksitas siklomatik di sini (gunakan metode sebelumnya atau metode lainnya)
            int nodeCount = 0;
            string[] lines = code.Split('\n');

            foreach (string line in lines)
            {
                string trimmedLine = line.Trim();

                if (trimmedLine.StartsWith("if") || trimmedLine.StartsWith("else") || trimmedLine.StartsWith("else if") || trimmedLine.StartsWith("while") ||
                    trimmedLine.StartsWith("for") || trimmedLine.StartsWith("foreach") || trimmedLine.StartsWith("case") || trimmedLine.StartsWith("catch") ||
                    trimmedLine.StartsWith("switch") || trimmedLine.StartsWith("try") || trimmedLine.Contains("main("))
                {
                    nodeCount++;
                }
            }
            return nodeCount + 1;
        }

        private int HitungKomponenTerhubung(string code)
        {
            int komponenterhubung = 0;

            // Membuat graf kontrol aliran dari kode sumber
            Dictionary<string, List<string>> kontrolAliranGraf = BuatGrafKontrolAliran(code);

            // Menyimpan node yang sudah dikunjungi
            HashSet<string> simpulDikunjungi = new HashSet<string>();

            foreach (var node in kontrolAliranGraf.Keys)
            {
                // Jika node belum dikunjungi, menambahkan komponen terhubung baru
                if (!simpulDikunjungi.Contains(node))
                {
                    penelusuran(node, kontrolAliranGraf, simpulDikunjungi);
                    komponenterhubung++;
                }
            }

            return komponenterhubung;
        }

        private Dictionary<string, List<string>> BuatGrafKontrolAliran(string code)
        {
            Dictionary<string, List<string>> kontrolAliranGraf = new Dictionary<string, List<string>>();
            string[] lines = code.Split('\n');

            foreach (string line in lines)
            {
                string trimmedLine = line.Trim();
                string currentNode = null;

                // Tambahkan node ke graf jika baris kode adalah percabangan atau loop
                if (trimmedLine.StartsWith("if") || trimmedLine.StartsWith("else") || trimmedLine.StartsWith("else if") || trimmedLine.StartsWith("while") ||
                    trimmedLine.StartsWith("for") || trimmedLine.StartsWith("foreach") || trimmedLine.StartsWith("case") || trimmedLine.StartsWith("catch") ||
                    trimmedLine.StartsWith("switch") || trimmedLine.StartsWith("try") || trimmedLine.Contains("main("))
                {
                    // Gunakan percabangan atau loop sebagai node
                    currentNode = trimmedLine;
                    kontrolAliranGraf[currentNode] = new List<string>();
                }

                // Menambahkan sisi ke graf jika terdapat keterhubungan antar node
                if (currentNode != null)
                {
                    foreach (var node in kontrolAliranGraf.Keys)
                    {
                        if (node != currentNode && trimmedLine.Contains(node))
                        {
                            kontrolAliranGraf[currentNode].Add(node);
                        }
                    }
                }
            }

            return kontrolAliranGraf;
        }
        // Konsep ini untuk menelusuri graf kontrol aliran dari kode sumber
        // Mengecek apakah apakah setiap simpul telah dikunjungi dan melanjutkan untuk menjelajahi setiap tetangga yang belum dikunjungi
        private void penelusuran(string node, Dictionary<string, List<string>> graph, HashSet<string> simpulDikunjungi)
        {
            simpulDikunjungi.Add(node);

            foreach (var simpultetangga in graph[node])
            {
                if (!simpulDikunjungi.Contains(simpultetangga))
                {
                    penelusuran(simpultetangga, graph, simpulDikunjungi);
                }
            }
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            string helpMessage = "This program is used to measure Line of Code, Cyclomatic Complexity, Essential, & Design\n" +
                                 //"You can upload files in these programming languages: C++, C#, Java, PHP.\n" +
                                 "You can upload files in these programming languages: C#\n\n" +

                                 "User guide:\n" +
                                 "1. Click 'Upload File' to select a source code file.\n" +
                                 "2. Click 'RUN' on the program you want to run.\n" +
                                 "3. Click 'Export .CSV' to save data in a CSV file\n" +
                                 "4. Click 'RESET' to clear all data in this feature\n\n" +

                                 "Instruction point:\n" +
                                 "<10       = low\n" +
                                 "10 - 20   = medium\n" +
                                 ">20       = high\n\n";                         
                                 
     
            MessageBox.Show(helpMessage, "Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            MMetric_UpldBox.Clear();
            txtOutput1.Clear();
            
            uploadedFilePath = string.Empty;
            // Clear any other relevant controls as needed
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(HMetric2_textBox.Text) && string.IsNullOrEmpty(HMetric3_textBox.Text) && string.IsNullOrEmpty(HMetric4_textBox.Text))
            {
                CSV_Button.Enabled = false;
            }

            if (!string.IsNullOrEmpty(MMetric_UpldBox.Text))
            {
                try
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "CSV Files (*.csv)|*.csv";
                    saveFileDialog.Title = "Save Metrics to CSV";
                    saveFileDialog.DefaultExt = "csv";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        // Open CSV file to write
                        using (StreamWriter sw = new StreamWriter(saveFileDialog.FileName))
                        {
                            // Write the headers                        
                            sw.WriteLine("Hasil perhitungan di box 1");
                            sw.WriteLine($"{txtOutput1.Text}");

                            sw.WriteLine("Hasil perhitungan di box 3");
                            sw.WriteLine($"{txtOutput3.Text}");

                            // Write the headers
                            //sw.WriteLine();
                            //sw.WriteLine("Perhitungan di box 2");
                            //sw.WriteLine($"{txtOutput2.Text}");                            

                            // Close the StreamWriter
                            sw.Close();
                        }

                        // Display a download confirmation message box
                        string message = "File successfully downloaded at : ";
                        string message2 = "Do you want to open the file?";
                        string title = "Confirmation";

                        // Show the MessageBox with OK and Cancel buttons
                        DialogResult result = MessageBox.Show(message + "\n" + saveFileDialog.FileName + "\n\n" + message2, title, MessageBoxButtons.OKCancel, MessageBoxIcon.Information);

                        // If the user clicks the OK button, open the CSV file
                        if (result == DialogResult.OK)
                        {
                            System.Diagnostics.Process.Start(saveFileDialog.FileName);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("No metrics to export", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnInfo1_Click(object sender, EventArgs e)
        {
            string Info1Message = "Cyclomatic complexity is software metric that provides a quantitative measure of the logical complexity of a program (Pressman2001)";

            MessageBox.Show(Info1Message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnInfo2_Click(object sender, EventArgs e)
        {
            string Info2Message = "to calculate cyclomatic complexity is as follows: \r\n\r\n" +
                "McCabe Cyclomatic Complexity ((V(G) = E - N + 2P)) and to calculate each matrix is as follows:\r\n\n" +
                "1. Counting the number of edges, you can count all the arrows or lines that connect the vertices in a control flow graph.\n" +
                "2. Counting A node in a control flow graph represents a point in the program at which execution can begin or end.\n" +
                "3. Calculating Connected Components is a group of nodes in a graph that are connected to each other, but not connected to nodes outside the group.";

            MessageBox.Show(Info2Message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        //END cyclomatic 


        //Start Essential Complexity
        private void MEssential_BtnRun_Click(object sender, EventArgs e)       
        {
                 
            // Periksa apakah pengguna telah memilih file
            if (string.IsNullOrEmpty(uploadedFilePath))
            {
                MessageBox.Show("Harap pilih file terlebih dahulu.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        
                
            string asu = File.ReadAllText(uploadedFilePath);

            // Hitung essential complexity
            int nonreduc = CountNonreduc(asu, out int A, out int B, out int C, out int D, out int E); // hitung jumlah node yang tidak dapat direduksi
            int entrypt = 1; // karena sudah ada method Main
            int termpt = 1; // karena aplikasi akan ditutup setelah selesai

            int essential = (nonreduc + 1) + (entrypt - 1) + (termpt - 1);


            // Tampilkan hasilnya di TextBox
            txtOutput1.Text = //$"INI LHO BOS hasil dari pencarian nonreduc : " + Environment.NewLine +
                            //$"{nonreduc}" + Environment.NewLine +
                            //$"{IsSignificantInitialization(asu)}" + Environment.NewLine +
                            //$"{IsResourceManagingObject(asu)}" + Environment.NewLine + Environment.NewLine +

                            $"Perhitungan NONREDUC" + Environment.NewLine +
                            $"Nilai A (deklarasi variabel yang mengelola obkel eksternal) : {A}" + Environment.NewLine +
                            $"Nilai B (variabel menginisialisasi nilai) : {B}" + Environment.NewLine +
                            $"Nilai C (proses penginputan kompleks) : {C}" + Environment.NewLine +
                            $"Nilai D (logika perulangan kompleks) : {D}" + Environment.NewLine +
                            $"Nilai E (Terdapat output yang berdampak signifikan) : {E}";

            txtOutput3.Text = $"essential = ( NONREDUC + 1 ) + ( ENTRYPT - 1 ) + ( TERMPT - 1 )" + Environment.NewLine +
                              $"essential = ( {nonreduc} + 1 ) + ( 1 - 1 ) + ( 1 - 1 )" + Environment.NewLine +
                              $"essential = {essential.ToString()}";


        }


        //tambahkan kode untuk menghitung essential disini, yang rumusnya (essential = ( NONREDUC + 1 ) + ( ENTRYPT - 1 ) + ( TERMPT - 1 ))
        private int CountNonreduc(string code, out int A, out int B, out int C, out int D, out int E)
        {
            int count = 0;

            A = 0;
            B = 0;
            C = 0;
            D = 0;
            E = 0;

            // Mencari deklarasi variabel
            string[] lines = code.Split('\n');
            foreach (string line in lines)
            {
                string trimmedLine = line.Trim();

                // Memeriksa apakah baris berisi deklarasi variabel
                if (trimmedLine.StartsWith("var") || trimmedLine.StartsWith("int") || trimmedLine.StartsWith("string") ||
                    trimmedLine.StartsWith("float") || trimmedLine.StartsWith("double") || trimmedLine.StartsWith("char"))
                {
                    // Memeriksa apakah deklarasi variabel adalah objek yang mengelola sumber daya eksternal
                    if (IsResourceManagingObject(trimmedLine))
                    {
                        count++;
                        A++;
                    }
                    // Memeriksa apakah deklarasi variabel menginisialisasi nilai yang memengaruhi jalannya program
                    else if (IsSignificantInitialization(trimmedLine))
                    {
                        count++;
                        B++;
                    }
                }
                // Memeriksa apakah terdapat proses penginputan yang memengaruhi jalannya program secara signifikan
                else if (trimmedLine.Contains("Console.ReadLine") || trimmedLine.Contains("ShowDialog"))
                {
                    count++;
                    C++;
                }
                // Memeriksa apakah terdapat logika dalam perulangan yang sangat kompleks
                else if (IsComplexLoopLogic(trimmedLine))
                {
                    count++;
                    D++;
                }
                // Memeriksa apakah terdapat operasi yang menampilkan hasil yang memicu perubahan besar dalam logika program
                else if (trimmedLine.Contains("Console.WriteLine") || trimmedLine.Contains("MessageBox.Show"))
                {
                    count++;
                    E++;

                }
            }


            return count;
        }

        private bool IsResourceManagingObject(string declaration)
        {
            // Daftar objek yang mungkin mengelola sumber daya eksternal
            string[] resourceObjects = { "FileStream", "StreamReader", "StreamWriter" };

            // Memeriksa apakah deklarasi variabel mengandung nama objek yang mengelola sumber daya eksternal
            foreach (string resourceObject in resourceObjects)
            {
                if (declaration.Contains(resourceObject))
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsSignificantInitialization(string declaration)
        {
            // Memeriksa apakah deklarasi variabel menginisialisasi nilai yang memengaruhi jalannya program secara signifikan
            // Misalnya, inisialisasi variabel dengan nilai yang berasal dari input pengguna
            return declaration.Contains("Console.ReadLine") || declaration.Contains("ShowDialog");
        }

        private bool IsComplexLoopLogic(string line)
        {
            // Memeriksa apakah logika dalam perulangan sangat kompleks
            // Misalnya, perulangan dengan banyak kondisi atau operasi yang rumit
            return line.Contains("for (") && (line.Contains("&&") || line.Contains("||") || line.Contains("=="));
        }

        //End essential cyclomatic


        //start Desgin cyclomatic
        private void BTN_DESIGN_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(uploadedFilePath))
            {
                var result = CalculateDesignComplexity(uploadedFilePath);
                DisplayResults(result);
            }
        }

        private (Dictionary<string, Module> modules, int totalComplexity, string mainModuleName) CalculateDesignComplexity(string filePath)
        {
            int totalComplexity = 0;
            var modules = new Dictionary<string, Module>();
            var moduleStack = new Stack<string>();
            string mainModuleName = string.Empty;

            try
            {
                var lines = File.ReadAllLines(filePath);

                foreach (var line in lines)
                {
                    string trimmedLine = line.Trim();
                    if (trimmedLine.StartsWith("class") || trimmedLine.StartsWith("void") || trimmedLine.StartsWith("int"))
                    {
                        string moduleName = ExtractModuleName(trimmedLine);
                        if (!string.IsNullOrEmpty(moduleName) && !modules.ContainsKey(moduleName))
                        {
                            modules[moduleName] = new Module(moduleName);
                            moduleStack.Push(moduleName);

                            // Mengidentifikasi modul utama sebagai modul pertama yang ditemukan
                            if (mainModuleName == string.Empty)
                            {
                                mainModuleName = moduleName;
                            }
                        }
                    }

                    if (IsControlStructure(trimmedLine))
                    {
                        if (moduleStack.Count > 0)
                        {
                            string currentModule = moduleStack.Peek();
                            modules[currentModule].Iv++;
                            modules[currentModule].ControlStructures.Add(trimmedLine);
                        }
                    }

                    if (trimmedLine.StartsWith("}"))
                    {
                        if (moduleStack.Count > 0)
                        {
                            moduleStack.Pop();
                        }
                    }
                }

                foreach (var module in modules.Values)
                {
                    totalComplexity += module.Iv;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading file: {ex.Message}");
            }

            return (modules, totalComplexity, mainModuleName);
        }

        private bool IsControlStructure(string line)
        {
            return line.StartsWith("if") ||
                   line.StartsWith("else if") ||
                   line.Contains("?") ||
                   line.StartsWith("for") ||
                   line.StartsWith("while") ||
                   line.StartsWith("do") ||
                   line.StartsWith("switch") ||
                   line.StartsWith("case") ||
                   line.StartsWith("try") ||
                   line.StartsWith("catch") ||
                   line.StartsWith("finally") ||
                   line.Contains("&&") ||
                   line.Contains("||");
        }

        private string ExtractModuleName(string line)
        {
            var parts = line.Trim().Split(' ');
            if (parts.Length > 1)
            {
                return parts[1].Split('(')[0];
            }
            return string.Empty;
        }

        private void DisplayResults((Dictionary<string, Module> modules, int totalComplexity, string mainModuleName) result)
        {
            var (modules, totalComplexity, mainModuleName) = result;

            txtOutput1.Clear();
            //textBox2.Clear();
            txtOutput3.Clear();


            // Menampilkan control structures setiap modul di textBox1
            txtOutput1.AppendText("Penemuan Iv dari tiap modul" + Environment.NewLine);
            foreach (var module in modules.Values)
            {
                txtOutput1.AppendText($"Modul {module.Name} = \"{string.Join(",  ", module.ControlStructures)}\"\n" + Environment.NewLine);
            }
            txtOutput1.AppendText(Environment.NewLine);

            txtOutput1.AppendText("Total Iv dari tiap modul" + Environment.NewLine);
            // Menampilkan perhitungan So setiap modul turunan di textBox2
            var moduleCalculations = new Dictionary<string, string>();
            foreach (var module in modules.Values)
            {
                if (module.Name != mainModuleName) // Pastikan modul utama tidak dimasukkan di textBox2
                {
                    var calculation = $"So({module.Name}) = iv({module.Iv})";
                    //txtOutput1.AppendText(Environment.NewLine);
                    txtOutput1.AppendText($"Modul {module.Name} = {calculation}\n" + Environment.NewLine);
                    moduleCalculations[module.Name] = calculation;
                }
            }

            // Menampilkan perhitungan So modul utama di textBox3
            var mainModule = modules.Values.FirstOrDefault(m => m.Name == mainModuleName);
            if (mainModule != null)
            {
                txtOutput3.AppendText("Jadi Total Dari Design Halstead\n" + Environment.NewLine);
                txtOutput3.AppendText($"Modul {mainModule.Name} (asu?)\n" + Environment.NewLine);

                // Mengumpulkan So dari modul-modul anak
                var childrenSoCalculations = mainModule.Children.Select(c => $"So({c.Name}) {c.CalculateSo()}").ToList();

                // Menampilkan perhitungan So modul UTAMA
                //var mainModuleCalculation = $"So({mainModule.Name}) = iv({mainModule.Iv}) + {string.Join(" + ", childrenSoCalculations)}";
                var mainModuleCalculation = $"So({mainModule.Name}) = iv({mainModule.Iv})";

                // Menambahkan perhitungan So modul turunan dari textBox2 ke textBox3
                foreach (var calculation in moduleCalculations)
                {
                    mainModuleCalculation += $" + {calculation.Value}";
                }

                mainModuleCalculation += $" = {totalComplexity}\n" + Environment.NewLine;
                txtOutput3.AppendText(mainModuleCalculation);
            }

        }

        //end design cyclomatic


        //button upload (halstead)
        private void Halstead_BtnUpload_Click(object sender, EventArgs e)
        {        
            OpenFileDialog openfile = new OpenFileDialog();
            // Set filter untuk file C dan C++
            //openfile.Filter = "C# Files (*.cs)|*.cs|Java Files (*.java)|*.java|C++ Files ( *.cpp)|*.cpp|PHP Files (*.php)|*.php";
            openfile.Filter = "C# Files (*.cs)|*.cs";

            DialogResult result = openfile.ShowDialog();

            if (result == DialogResult.OK)
            {
                uploadedFilePath = openfile.FileName;
                HMetric_textBox.Text = openfile.FileName;
            }
        }

        //button clear run 
       
                
        //button reset
        private void Halstead_BtnReset_Click(object sender, EventArgs e)
        {
            HMetric_textBox.Clear();
            HMetric2_textBox.Clear();
            HMetric3_textBox.Clear();
            HMetric4_textBox.Clear();
            uploadedFilePath = string.Empty;
        }

        //button export .csv
        private void CSV_Button_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(HMetric2_textBox.Text) && string.IsNullOrEmpty(HMetric3_textBox.Text) && string.IsNullOrEmpty(HMetric4_textBox.Text))
            {
                CSV_Button.Enabled = false;
            }

            if (!string.IsNullOrEmpty(HMetric_textBox.Text))
            {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "CSV Files (*.csv)|*.csv";
                saveFileDialog.Title = "Save Metrics to CSV";
                saveFileDialog.DefaultExt = "csv";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Open CSV file to write
                    using (StreamWriter sw = new StreamWriter(saveFileDialog.FileName))
                    {
                        // Write the headers                        
                        sw.WriteLine("Perhitungan di box 1");
                        sw.WriteLine($"{HMetric2_textBox.Text}");

                        // Write the headers
                        sw.WriteLine();
                        sw.WriteLine("Perhitungan di box 2");
                        sw.WriteLine($"{HMetric3_textBox.Text}");
                            

                        // Write box 3 content
                        sw.WriteLine();
                        sw.WriteLine("Hasil hitungnya di box 3");
                        sw.WriteLine(HMetric4_textBox.Text);

                        // Close the StreamWriter
                        sw.Close();
                    }

                        // Display a download confirmation message box
                        string message = "File successfully downloaded at : ";
                        string message2 = "Do you want to open the file?";
                        string title = "Confirmation";

                        // Show the MessageBox with OK and Cancel buttons
                        DialogResult result = MessageBox.Show(message + "\n" + saveFileDialog.FileName + "\n\n" + message2, title, MessageBoxButtons.OKCancel, MessageBoxIcon.Information);

                        // If the user clicks the OK button, open the CSV file
                        if (result == DialogResult.OK)
                        {
                            System.Diagnostics.Process.Start(saveFileDialog.FileName);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("No metrics to export", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        //start priv dictionary (menghitung operator dan operand unik) 
        private Dictionary<string, string> ExtractVariablesAndValues(string code)
        {
            Dictionary<string, string> variablesAndValues = new Dictionary<string, string>();

            // Ekstrak variabel dan nilai variabel menggunakan ekspresi reguler
            //MatchCollection matches = Regex.Matches(code, @"\b(int|string|float|bool)\b\s+([a-zA-Z_]\w*)\s*=\s*([^;]+);");

            //baris dibawah ini yang jadi bikin operand kurang akurat
            //MatchCollection matches = Regex.Matches(code, @"(?<var>\b\w+\b)\s*=\s*(?<val>[^;]+);");

            MatchCollection matches = Regex.Matches(code, @"\b(int|string|float|bool)\b\s+(?<var>[a-zA-Z_]\w*)\s*=\s*(?<val>[^;]+);");


            foreach (Match match in matches)
            {
                //string variable = match.Groups[2].Value;
                //string value = match.Groups[3].Value.Trim();

                string variable = match.Groups["var"].Value;
                string value = match.Groups["val"].Value.Trim();


                if (!variablesAndValues.ContainsKey(variable))
                {
                    variablesAndValues.Add(variable, value);
                }
                else
                {
                    variablesAndValues[variable] += $", {value}";
                }

                // Identifikasi dan tambahkan operasi increment dan decrement ke variabel
                string[] incrementDecrementOps = { "++", "--" };
                foreach (string op in incrementDecrementOps)
                {
                    string incrementDecrementPattern = $@"(?<![+\-])\b{variable}\s*{Regex.Escape(op)}";
                    MatchCollection incrementDecrementMatches = Regex.Matches(code, incrementDecrementPattern);
                    foreach (Match incrementDecrementMatch in incrementDecrementMatches)
                    {
                        variablesAndValues[variable] += $", {op}";
                    }
                }

            }


            // Hitung total dari semua nilai yang dimiliki oleh variabel
            int totalValues = variablesAndValues.Values.SelectMany(v => v.Split(',')).Distinct().Count();

            // Tambahkan total nilai variabel ke dalam hitungan N2
            //int N2 = totalValues;

            return variablesAndValues;
        }
        //end priv dictionary (buat ekstrak var dan val yang dibutuhkan) 


        //Start program length (halstead)
        private void HProLenght_BtnRun_Click(object sender, EventArgs e)
        {

            // Periksa apakah pengguna telah memilih file
            if (string.IsNullOrEmpty(uploadedFilePath))
            {
                MessageBox.Show("Harap pilih file terlebih dahulu.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Membaca seluruh isi file
            string code = System.IO.File.ReadAllText(uploadedFilePath);

            // Daftar operator
            string[] operators = { "+", "-", "*", "/", "%", "&", "|", "^", "!", "~",
                "=", "!=", ">", "<", ">=", "<=", "==", "<<", ">>",
                "&&", "||", "?", ":", ";", "{", "}", "[", "]", "(", ")", ".", ",",
                "++", "--", "+=", "-=", "*=", "/=", "%=", "&=", "|=", "^=", "<<=", ">>=",
                "&&=", "||=", "=>", "==", "!=", "<", ">", ">=", "<=", "===", "!==",
                "is", "as", "in", "out", "ref", "params", "unchecked", "checked",
                "throw", "try", "catch", "finally", "for", "while", "do",
                "switch", "case", "break", "continue", "default", "goto", "return",
                "if", "else", "else if", "foreach", "using", "lock", "try", "catch", "finally" }; // Menambahkan if, else, dll.

            // Menggunakan Dictionary untuk menghitung operator dan operand unik
            Dictionary<string, int> uniqueOperators = new Dictionary<string, int>();

            // Menghitung jumlah operator
            foreach (string token in code.Split(' '))
            {
                if (operators.Contains(token))
                {
                    if (uniqueOperators.ContainsKey(token))
                        uniqueOperators[token]++;
                    else
                        uniqueOperators[token] = 1;
                }
            }

            // Membersihkan konten sebelum menampilkan hasilnya
            HMetric2_textBox.Clear();
            HMetric3_textBox.Clear();
            HMetric4_textBox.Clear();

            // Ekstraksi variabel dan nilai variabel dari kode
            var variablesAndValues = ExtractVariablesAndValues(code);
            

            // Menampilkan jumlah total token di TextBox3
            int N1 = uniqueOperators.Values.Sum();
            //int N1 = uniqueOperators.Values.Sum(); //buat HVocab
            int N2 = variablesAndValues.Count + variablesAndValues.SelectMany(v => v.Value.Split(',')).Count();
            //int N2 = variablesAndValues.Count; // Menggunakan jumlah unik variabel yang diekstraksi //BUAT HVOCAB
            int N = N1 + N2;
            HMetric4_textBox.Text = $"Total operator (N1): {N1}, Total operand (N2): {N2} " + Environment.NewLine + $"Total Jumlah operator dan operand N = N1 + N2 ({N})";


            // Menampilkan hasil perhitungan N1 di TextBox2
            HMetric2_textBox.AppendText("Jumlah Operator Unik (N1):" + Environment.NewLine);
            foreach (var pair in uniqueOperators)
            {
                HMetric2_textBox.AppendText($"{pair.Key} : {pair.Value}" + Environment.NewLine);
            }


            // Menampilkan hasil ekstraksi variabel di TextBox3
            HMetric3_textBox.AppendText("Jumlah Operand Unik (N2):" + Environment.NewLine);
            foreach (var pair in variablesAndValues)
            {
                HMetric3_textBox.AppendText($"{pair.Key} : {pair.Value}" + Environment.NewLine);
            }

        }        

        //End program length (halstead)


        //start vocabulary (halstead)
        private void HVocab_BtnRun_Click(object sender, EventArgs e)
        {
            // Periksa apakah pengguna telah memilih file
            if (string.IsNullOrEmpty(uploadedFilePath))
            {
                MessageBox.Show("Harap pilih file terlebih dahulu.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Membaca seluruh isi file
            string code = System.IO.File.ReadAllText(uploadedFilePath);

            // Daftar operator
            string[] operators = { "+", "-", "*", "/", "%", "&", "|", "^", "!", "~",
            "=", "!=", ">", "<", ">=", "<=", "==", "<<", ">>",
            "&&", "||", "?", ":", ";", "{", "}", "[", "]", "(", ")", ".", ",",
            "++", "--", "+=", "-=", "*=", "/=", "%=", "&=", "|=", "^=", "<<=", ">>=",
            "&&=", "||=", "=>", "==", "!=", "<", ">", ">=", "<=", "===", "!==",
            "is", "as", "in", "out", "ref", "params", "unchecked", "checked",
            "throw", "try", "catch", "finally", "for", "while", "do",
            "switch", "case", "break", "continue", "default", "goto", "return",
            "if", "else", "else if", "foreach", "using", "lock", "try", "catch", "finally" }; // Menambahkan if, else, dll.

            // Menggunakan Dictionary untuk menghitung operator dan operand unik
            Dictionary<string, int> uniqueOperators = new Dictionary<string, int>();

            // Menghitung jumlah operator
            foreach (string token in code.Split(' '))
            {
                if (operators.Contains(token))
                {
                    if (uniqueOperators.ContainsKey(token))
                        uniqueOperators[token]++;
                    else
                        uniqueOperators[token] = 1;
                }
            }

            // Membersihkan konten sebelum menampilkan hasilnya
            HMetric2_textBox.Clear();
            HMetric3_textBox.Clear();
            HMetric4_textBox.Clear();

            // Ekstraksi variabel dan nilai variabel dari kode
            var variablesAndValues = ExtractVariablesAndValues(code);
            
            // Menampilkan jumlah total token di TextBox3
            //int N1 = uniqueOperators.Values.Sum(); //buat program length
            //int n1 = uniqueOperators.Values.Sum(); //buat HVocab (menghitung valuenya, bukan variabelnya)
            int n1 = uniqueOperators.Count;

            //int N2 = variablesAndValues.Count + variablesAndValues.SelectMany(v => v.Value.Split(',')).Count(); //buat program length
            int n2 = variablesAndValues.Count; // Menggunakan jumlah unik variabel yang diekstraksi //BUAT HVOCAB
            int n = n1 + n2;
            HMetric4_textBox.Text = $"Total operator (n1): {n1}, Total operand (n2): {n2} " + Environment.NewLine + $"Total operator & operand n = n1 + n2 ({n})";
            
            
            // Menampilkan hasil perhitungan N1 di TextBox2
            HMetric2_textBox.AppendText("Operator Unik (n1):" + Environment.NewLine);
            foreach (var pair in uniqueOperators)
            {
                HMetric2_textBox.AppendText($"{pair.Key}" + Environment.NewLine);
            }


            // Menampilkan hasil ekstraksi variabel di TextBox3
            HMetric3_textBox.AppendText("Operand Unik (n2):" + Environment.NewLine);
            foreach (var pair in variablesAndValues)
            {
                HMetric3_textBox.AppendText($"{pair.Key}" + Environment.NewLine);
            }

        }

        //end vocabulary (halstead)


        //start volume (halstead)
        private void HVol_BtnRun_Click(object sender, EventArgs e)
        {
            // Periksa apakah pengguna telah memilih file
            if (string.IsNullOrEmpty(uploadedFilePath))
            {
                MessageBox.Show("Harap pilih file terlebih dahulu.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Membaca seluruh isi file
            string code = System.IO.File.ReadAllText(uploadedFilePath);

            // Daftar operator
            string[] operators = { "+", "-", "*", "/", "%", "&", "|", "^", "!", "~",
            "=", "!=", ">", "<", ">=", "<=", "==", "<<", ">>",
            "&&", "||", "?", ":", ";", "{", "}", "[", "]", "(", ")", ".", ",",
            "++", "--", "+=", "-=", "*=", "/=", "%=", "&=", "|=", "^=", "<<=", ">>=",
            "&&=", "||=", "=>", "==", "!=", "<", ">", ">=", "<=", "===", "!==",
            "is", "as", "in", "out", "ref", "params", "unchecked", "checked",
            "throw", "try", "catch", "finally", "for", "while", "do",
            "switch", "case", "break", "continue", "default", "goto", "return",
            "if", "else", "else if", "foreach", "using", "lock", "try", "catch", "finally" }; // Menambahkan if, else, dll.

            // Menggunakan Dictionary untuk menghitung operator dan operand unik
            Dictionary<string, int> uniqueOperators = new Dictionary<string, int>();

            // Menghitung jumlah operator
            foreach (string token in code.Split(' '))
            {
                if (operators.Contains(token))
                {
                    if (uniqueOperators.ContainsKey(token))
                        uniqueOperators[token]++;
                    else
                        uniqueOperators[token] = 1;
                }
            }

            // Membersihkan konten sebelum menampilkan hasilnya
            HMetric2_textBox.Clear();
            HMetric3_textBox.Clear();
            HMetric4_textBox.Clear();

            // Ekstraksi variabel dan nilai variabel dari kode
            var variablesAndValues = ExtractVariablesAndValues(code);

            // Menampilkan jumlah total token di TextBox3
            int N1 = uniqueOperators.Values.Sum(); //buat program length
            //int n1 = uniqueOperators.Values.Sum(); //buat HVocab (menghitung valuenya, bukan variabelnya)
            int n1 = uniqueOperators.Count;

            int N2 = variablesAndValues.Count + variablesAndValues.SelectMany(v => v.Value.Split(',')).Count(); //buat program length
            int n2 = variablesAndValues.Count; // Menggunakan jumlah unik variabel yang diekstraksi //BUAT HVOCAB

            int n = n1 + n2;
            int N = N1 + N2;
            double V = N * Math.Log(n, 2); //volumeHalstead

            //HMetric4_textBox.Text = $"Total operator (N1): {n1}, Total operand (N2): {n2}, Total token N = N1 + N2 ({n})";
            HMetric4_textBox.Text = ($"Total N = {N}, Total n = {n}" + Environment.NewLine + $"Total volume halstead, V = N * log2(n) ({V})");

            // Menampilkan hasil perhitungan n1 di TextBox2
            HMetric2_textBox.AppendText("Operator Unik (n1) =" + Environment.NewLine);
            foreach (var pair in uniqueOperators)
            {
                HMetric2_textBox.AppendText($"{pair.Key} " + Environment.NewLine);

            }
            // Menampilkan hasil perhitungan n2 di TextBox2
            HMetric2_textBox.AppendText(Environment.NewLine + "Operand Unik (n2):" + Environment.NewLine);
            foreach (var pair2 in variablesAndValues)
            {
                HMetric2_textBox.AppendText($"{pair2.Key}" + Environment.NewLine);
            }


            // Menampilkan hasil perhitungan N1 di TextBox3
            HMetric3_textBox.AppendText("Operator Unik (N1) =" + Environment.NewLine);
            foreach (var pair in uniqueOperators)
            {
                HMetric3_textBox.AppendText($"{pair.Key} : {pair.Value}" + Environment.NewLine);

            }
            // Menampilkan hasil perhitungan N2 di TextBox3
            HMetric3_textBox.AppendText(Environment.NewLine + "Operand Unik (N2):" + Environment.NewLine);
            foreach (var pair2 in variablesAndValues)
            {
                HMetric3_textBox.AppendText($"{pair2.Key} : {pair2.Value}" + Environment.NewLine);
            }

        }

        //end volume (halstead)


        //start difficulty (halstead)
        private void HDiff_BtnRun_Click(object sender, EventArgs e)
        {
            // Periksa apakah pengguna telah memilih file
            if (string.IsNullOrEmpty(uploadedFilePath))
            {
                MessageBox.Show("Harap pilih file terlebih dahulu.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Membaca seluruh isi file
            string code = System.IO.File.ReadAllText(uploadedFilePath);

            // Daftar operator
            string[] operators = { "+", "-", "*", "/", "%", "&", "|", "^", "!", "~",
            "=", "!=", ">", "<", ">=", "<=", "==", "<<", ">>",
            "&&", "||", "?", ":", ";", "{", "}", "[", "]", "(", ")", ".", ",",
            "++", "--", "+=", "-=", "*=", "/=", "%=", "&=", "|=", "^=", "<<=", ">>=",
            "&&=", "||=", "=>", "==", "!=", "<", ">", ">=", "<=", "===", "!==",
            "is", "as", "in", "out", "ref", "params", "unchecked", "checked",
            "throw", "try", "catch", "finally", "for", "while", "do",
            "switch", "case", "break", "continue", "default", "goto", "return",
            "if", "else", "else if", "foreach", "using", "lock", "try", "catch", "finally" }; // Menambahkan if, else, dll.

            // Menggunakan Dictionary untuk menghitung operator dan operand unik
            Dictionary<string, int> uniqueOperators = new Dictionary<string, int>();

            // Menghitung jumlah operator
            foreach (string token in code.Split(' '))
            {
                if (operators.Contains(token))
                {
                    if (uniqueOperators.ContainsKey(token))
                        uniqueOperators[token]++;
                    else
                        uniqueOperators[token] = 1;
                }
            }

            // Membersihkan konten sebelum menampilkan hasilnya
            HMetric2_textBox.Clear();
            HMetric3_textBox.Clear();
            HMetric4_textBox.Clear();

            // Ekstraksi variabel dan nilai variabel dari kode
            var variablesAndValues = ExtractVariablesAndValues(code);

            // Menampilkan jumlah total token di TextBox3
            int N1 = uniqueOperators.Values.Sum(); //buat program length
            //int n1 = uniqueOperators.Values.Sum(); //buat HVocab (menghitung valuenya, bukan variabelnya)
            int n1 = uniqueOperators.Count;

            //kalau pakai int nanti koma dibelakangnya ngak ke itung misal 3,75 nah 0,75 nya ngak ke hitung (bisa pakai float atau double udah aku coba)
            double N2 = variablesAndValues.Count + variablesAndValues.SelectMany(v => v.Value.Split(',')).Count(); //buat program length
            double n2 = variablesAndValues.Count; // Menggunakan jumlah unik variabel yang diekstraksi //BUAT HVOCAB

            double n = n1 + n2;
            double N = N1 + N2;
            double V = N * Math.Log(n, 2); //volumeHalstead
            double D = (n1 / 2) * (N2 / n2);            

            //menghitung TOTAL
            HMetric4_textBox.AppendText
            ($"Difficulyty, D  = ( n1 / 2 ) * ( N2 / n2 )" + Environment.NewLine +
            $"Difficulyty, D  = ( {n1} / 2 ) * ( {N2} / {n2} )" + Environment.NewLine +
            $"Total Difficulyty, D  = {D}");


            // Menampilkan hasil perhitungan n1 di TextBox2
            HMetric2_textBox.AppendText("Operator Unik (n1) =" + Environment.NewLine);
            foreach (var pair in uniqueOperators)
            {
                HMetric2_textBox.AppendText($"{pair.Key} " + Environment.NewLine);
                
            }
            // Menampilkan hasil perhitungan n2 di TextBox2
            HMetric2_textBox.AppendText(Environment.NewLine + "Operand Unik (n2):" + Environment.NewLine);
            foreach (var pair2 in variablesAndValues)
            {
                HMetric2_textBox.AppendText
                ($"{pair2.Key}" + Environment.NewLine);
            }



            // Menampilkan hasil perhitungan N2 di TextBox3
            HMetric3_textBox.AppendText("Jumlah Operand Unik (N2):" + Environment.NewLine);
            foreach (var pair in variablesAndValues)
            {
                HMetric3_textBox.AppendText($"{pair.Key} : {pair.Value}" + Environment.NewLine);               
            }

        }
        //end difficulty (halstead)


        //start effort (halstead)
        private void HEffort_BtnRun_Click(object sender, EventArgs e)
        {
            // Periksa apakah pengguna telah memilih file
            if (string.IsNullOrEmpty(uploadedFilePath))
            {
                MessageBox.Show("Harap pilih file terlebih dahulu.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Membaca seluruh isi file
            string code = System.IO.File.ReadAllText(uploadedFilePath);

            // Daftar operator
            string[] operators = { "+", "-", "*", "/", "%", "&", "|", "^", "!", "~",
            "=", "!=", ">", "<", ">=", "<=", "==", "<<", ">>",
            "&&", "||", "?", ":", ";", "{", "}", "[", "]", "(", ")", ".", ",",
            "++", "--", "+=", "-=", "*=", "/=", "%=", "&=", "|=", "^=", "<<=", ">>=",
            "&&=", "||=", "=>", "==", "!=", "<", ">", ">=", "<=", "===", "!==",
            "is", "as", "in", "out", "ref", "params", "unchecked", "checked",
            "throw", "try", "catch", "finally", "for", "while", "do",
            "switch", "case", "break", "continue", "default", "goto", "return",
            "if", "else", "else if", "foreach", "using", "lock", "try", "catch", "finally" }; // Menambahkan if, else, dll.

            // Menggunakan Dictionary untuk menghitung operator dan operand unik
            Dictionary<string, int> uniqueOperators = new Dictionary<string, int>();

            // Menghitung jumlah operator
            foreach (string token in code.Split(' '))
            {
                if (operators.Contains(token))
                {
                    if (uniqueOperators.ContainsKey(token))
                        uniqueOperators[token]++;
                    else
                        uniqueOperators[token] = 1;
                }
            }

            // Membersihkan konten sebelum menampilkan hasilnya
            HMetric2_textBox.Clear();
            HMetric3_textBox.Clear();
            HMetric4_textBox.Clear();

            // Ekstraksi variabel dan nilai variabel dari kode
            var variablesAndValues = ExtractVariablesAndValues(code);

            // Menampilkan jumlah total token di TextBox3
            int N1 = uniqueOperators.Values.Sum(); //buat program length
            //int n1 = uniqueOperators.Values.Sum(); //buat HVocab (menghitung valuenya, bukan variabelnya)
            int n1 = uniqueOperators.Count;

            //kalau pakai int nanti koma dibelakangnya ngak ke itung misal 3,75 nah 0,75 nya ngak ke hitung (bisa pakai float atau double udah aku coba)
            double N2 = variablesAndValues.Count + variablesAndValues.SelectMany(v => v.Value.Split(',')).Count(); //buat program length
            double n2 = variablesAndValues.Count; // Menggunakan jumlah unik variabel yang diekstraksi //BUAT HVOCAB

            double n = n1 + n2;
            double N = N1 + N2;
            double V = N * Math.Log(n, 2); //volumeHalstead
            double D = (n1 / 2) * (N2 / n2);
            double E = V * D;

            //menghitung TOTAL
            HMetric4_textBox.AppendText
            ($"Effort, E = Volume (V) * Difficulyty (D)" + Environment.NewLine +
            $"Effort, E = Volume ({V}) * Difficulyty ({D})" + Environment.NewLine +
            $"Effort, E = {E}");

            //V (masuk ke text box 2)
            // Menampilkan hasil perhitungan n1 di TextBox2
            HMetric2_textBox.AppendText("Operator Unik (n1) =" + Environment.NewLine);
            foreach (var pair in uniqueOperators)
            {
                HMetric2_textBox.AppendText($"{pair.Key} " + Environment.NewLine);

            }
            // Menampilkan hasil perhitungan n2 di TextBox2
            HMetric2_textBox.AppendText(Environment.NewLine + "Operand Unik (n2):" + Environment.NewLine);
            foreach (var pair in variablesAndValues)
            {
                HMetric2_textBox.AppendText($"{pair.Key}" + Environment.NewLine);
            }
            // Menampilkan hasil perhitungan N1 di TextBox3
            HMetric2_textBox.AppendText(Environment.NewLine + "Operator Unik (N1) =" + Environment.NewLine);
            foreach (var pair in uniqueOperators)
            {
                HMetric2_textBox.AppendText($"{pair.Key} : {pair.Value}" + Environment.NewLine);

            }
            // Menampilkan hasil perhitungan N2 di TextBox3
            HMetric2_textBox.AppendText(Environment.NewLine + "Operand Unik (N2):" + Environment.NewLine);
            foreach (var pair in variablesAndValues)
            {
                HMetric2_textBox.AppendText($"{pair.Key} : {pair.Value}" + Environment.NewLine);
            }


            // D (masuk text box 3)
            // Menampilkan hasil perhitungan n1 di TextBox2
            HMetric3_textBox.AppendText("Operator Unik (n1) =" + Environment.NewLine);
            foreach (var pair in uniqueOperators)
            {
                HMetric3_textBox.AppendText($"{pair.Key} " + Environment.NewLine);

            }
            // Menampilkan hasil perhitungan n2 di TextBox2
            HMetric3_textBox.AppendText(Environment.NewLine + "Operand Unik (n2):" + Environment.NewLine);
            foreach (var pair in variablesAndValues)
            {
                HMetric3_textBox.AppendText($"{pair.Key}" + Environment.NewLine);
            }
            // Menampilkan hasil perhitungan N2 di TextBox3
            HMetric3_textBox.AppendText(Environment.NewLine + "Jumlah Operand Unik (N2):" + Environment.NewLine);
            foreach (var pair in variablesAndValues)
            {
                HMetric3_textBox.AppendText($"{pair.Key} : {pair.Value}" + Environment.NewLine);
            }

        }
        //end effort (halstead)


        //start time estimator (halstead)
        private void HTimeEst_BtnRun_Click(object sender, EventArgs e)
        {
            // Periksa apakah pengguna telah memilih file
            if (string.IsNullOrEmpty(uploadedFilePath))
            {
                MessageBox.Show("Harap pilih file terlebih dahulu.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Membaca seluruh isi file
            string code = System.IO.File.ReadAllText(uploadedFilePath);

            // Daftar operator
            string[] operators = { "+", "-", "*", "/", "%", "&", "|", "^", "!", "~",
            "=", "!=", ">", "<", ">=", "<=", "==", "<<", ">>",
            "&&", "||", "?", ":", ";", "{", "}", "[", "]", "(", ")", ".", ",",
            "++", "--", "+=", "-=", "*=", "/=", "%=", "&=", "|=", "^=", "<<=", ">>=",
            "&&=", "||=", "=>", "==", "!=", "<", ">", ">=", "<=", "===", "!==",
            "is", "as", "in", "out", "ref", "params", "unchecked", "checked",
            "throw", "try", "catch", "finally", "for", "while", "do",
            "switch", "case", "break", "continue", "default", "goto", "return",
            "if", "else", "else if", "foreach", "using", "lock", "try", "catch", "finally" }; // Menambahkan if, else, dll.

            // Menggunakan Dictionary untuk menghitung operator dan operand unik
            Dictionary<string, int> uniqueOperators = new Dictionary<string, int>();

            // Menghitung jumlah operator
            foreach (string token in code.Split(' '))
            {
                if (operators.Contains(token))
                {
                    if (uniqueOperators.ContainsKey(token))
                        uniqueOperators[token]++;
                    else
                        uniqueOperators[token] = 1;
                }
            }

            // Membersihkan konten sebelum menampilkan hasilnya
            HMetric2_textBox.Clear();
            HMetric3_textBox.Clear();
            HMetric4_textBox.Clear();

            // Ekstraksi variabel dan nilai variabel dari kode
            var variablesAndValues = ExtractVariablesAndValues(code);

            // Menampilkan jumlah total token di TextBox3
            int N1 = uniqueOperators.Values.Sum(); //buat program length
            //int n1 = uniqueOperators.Values.Sum(); //buat HVocab (menghitung valuenya, bukan variabelnya)
            int n1 = uniqueOperators.Count;

            //kalau pakai int nanti koma dibelakangnya ngak ke itung misal 3,75 nah 0,75 nya ngak ke hitung (bisa pakai float atau double udah aku coba)
            double N2 = variablesAndValues.Count + variablesAndValues.SelectMany(v => v.Value.Split(',')).Count(); //buat program length
            double n2 = variablesAndValues.Count; // Menggunakan jumlah unik variabel yang diekstraksi //BUAT HVOCAB

            double n = n1 + n2;
            double N = N1 + N2;
            double V = N * Math.Log(n, 2); //volumeHalstead
            double D = (n1 / 2) * (N2 / n2);
            double E = V * D;
            double TE = n1 * Math.Log(n1, 2) + n2 * Math.Log(n2, 2);


            HMetric4_textBox.Text = 
                ($"Time Estimator, TE = n1log2n1 + n2log2n2 " + Environment.NewLine +
                $"Time Estimator, TE = {n1} log2 {n1} + {n2} log2 {n2} " + Environment.NewLine +
                $"Time Estimator, TE = {TE}");


            // Menampilkan hasil perhitungan N1 di TextBox2
            HMetric2_textBox.AppendText("Operator Unik (n1):" + Environment.NewLine);
            foreach (var pair in uniqueOperators)
            {
                HMetric2_textBox.AppendText($"{pair.Key}" + Environment.NewLine);
            }


            // Menampilkan hasil ekstraksi variabel di TextBox3
            HMetric3_textBox.AppendText("Operand Unik (n2):" + Environment.NewLine);
            foreach (var pair in variablesAndValues)
            {
                HMetric3_textBox.AppendText($"{pair.Key}" + Environment.NewLine);
            }


        }

        //end time estimator (halstead)


        //start intelligence (halstead)
        private void HIntel_BtnRun_Click(object sender, EventArgs e)
        {
            // Periksa apakah pengguna telah memilih file
            if (string.IsNullOrEmpty(uploadedFilePath))
            {
                MessageBox.Show("Harap pilih file terlebih dahulu.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Membaca seluruh isi file
            string code = System.IO.File.ReadAllText(uploadedFilePath);

            // Daftar operator
            string[] operators = { "+", "-", "*", "/", "%", "&", "|", "^", "!", "~",
            "=", "!=", ">", "<", ">=", "<=", "==", "<<", ">>",
            "&&", "||", "?", ":", ";", "{", "}", "[", "]", "(", ")", ".", ",",
            "++", "--", "+=", "-=", "*=", "/=", "%=", "&=", "|=", "^=", "<<=", ">>=",
            "&&=", "||=", "=>", "==", "!=", "<", ">", ">=", "<=", "===", "!==",
            "is", "as", "in", "out", "ref", "params", "unchecked", "checked",
            "throw", "try", "catch", "finally", "for", "while", "do",
            "switch", "case", "break", "continue", "default", "goto", "return",
            "if", "else", "else if", "foreach", "using", "lock", "try", "catch", "finally" }; // Menambahkan if, else, dll.

            // Menggunakan Dictionary untuk menghitung operator dan operand unik
            Dictionary<string, int> uniqueOperators = new Dictionary<string, int>();

            // Menghitung jumlah operator
            foreach (string token in code.Split(' '))
            {
                if (operators.Contains(token))
                {
                    if (uniqueOperators.ContainsKey(token))
                        uniqueOperators[token]++;
                    else
                        uniqueOperators[token] = 1;
                }
            }

            // Membersihkan konten sebelum menampilkan hasilnya
            HMetric2_textBox.Clear();
            HMetric3_textBox.Clear();
            HMetric4_textBox.Clear();

            // Ekstraksi variabel dan nilai variabel dari kode
            var variablesAndValues = ExtractVariablesAndValues(code);

            // Menampilkan jumlah total token di TextBox3
            int N1 = uniqueOperators.Values.Sum(); //buat program length
            //int n1 = uniqueOperators.Values.Sum(); //buat HVocab (menghitung valuenya, bukan variabelnya)
            int n1 = uniqueOperators.Count;

            //kalau pakai int nanti koma dibelakangnya ngak ke itung misal 3,75 nah 0,75 nya ngak ke hitung (bisa pakai float atau double udah aku coba)
            double N2 = variablesAndValues.Count + variablesAndValues.SelectMany(v => v.Value.Split(',')).Count(); //buat program length
            double n2 = variablesAndValues.Count; // Menggunakan jumlah unik variabel yang diekstraksi //BUAT HVOCAB

            double n = n1 + n2;
            double N = N1 + N2;
            double V = N * Math.Log(n, 2); //volumeHalstead
            double D = (n1 / 2) * (N2 / n2);
            double E = V * D;
            double I = V / D;

            //menghitung TOTAL
            HMetric4_textBox.AppendText
            ($"intelligence, I = Volume (V) / Difficulyty (D)" + Environment.NewLine +
            $"intelligence, I = Volume ({V}) / Difficulyty ({D})" + Environment.NewLine +
            $"intelligence, I = {I}");

            //V (masuk ke text box 2)
            // Menampilkan hasil perhitungan n1 di TextBox2
            HMetric2_textBox.AppendText("Operator Unik (n1) =" + Environment.NewLine);
            foreach (var pair in uniqueOperators)
            {
                HMetric2_textBox.AppendText($"{pair.Key} " + Environment.NewLine);

            }
            // Menampilkan hasil perhitungan n2 di TextBox2
            HMetric2_textBox.AppendText(Environment.NewLine + "Operand Unik (n2):" + Environment.NewLine);
            foreach (var pair in variablesAndValues)
            {
                HMetric2_textBox.AppendText($"{pair.Key}" + Environment.NewLine);
            }
            // Menampilkan hasil perhitungan N1 di TextBox3
            HMetric2_textBox.AppendText(Environment.NewLine + "Operator Unik (N1) =" + Environment.NewLine);
            foreach (var pair in uniqueOperators)
            {
                HMetric2_textBox.AppendText($"{pair.Key} : {pair.Value}" + Environment.NewLine);

            }
            // Menampilkan hasil perhitungan N2 di TextBox3
            HMetric2_textBox.AppendText(Environment.NewLine + "Operand Unik (N2):" + Environment.NewLine);
            foreach (var pair in variablesAndValues)
            {
                HMetric2_textBox.AppendText($"{pair.Key} : {pair.Value}" + Environment.NewLine);
            }


            // D (masuk text box 3)
            // Menampilkan hasil perhitungan n1 di TextBox2
            HMetric3_textBox.AppendText("Operator Unik (n1) =" + Environment.NewLine);
            foreach (var pair in uniqueOperators)
            {
                HMetric3_textBox.AppendText($"{pair.Key} " + Environment.NewLine);

            }
            // Menampilkan hasil perhitungan n2 di TextBox2
            HMetric3_textBox.AppendText(Environment.NewLine + "Operand Unik (n2):" + Environment.NewLine);
            foreach (var pair in variablesAndValues)
            {
                HMetric3_textBox.AppendText($"{pair.Key}" + Environment.NewLine);
            }
            // Menampilkan hasil perhitungan N2 di TextBox3
            HMetric3_textBox.AppendText(Environment.NewLine + "Jumlah Operand Unik (N2):" + Environment.NewLine);
            foreach (var pair in variablesAndValues)
            {
                HMetric3_textBox.AppendText($"{pair.Key} : {pair.Value}" + Environment.NewLine);
            }

        }

        //endintelligence (halstead)



        //Start line count (halstead)
        private void HLineCount_BtnRun_Click(object sender, EventArgs e)
        {
            // Membersihkan konten sebelum menampilkan hasilnya
            HMetric2_textBox.Clear();
            HMetric3_textBox.Clear();
            HMetric4_textBox.Clear();

            if (!string.IsNullOrEmpty(uploadedFilePath) && File.Exists(uploadedFilePath))
            {
                string inicodingnya = File.ReadAllText(uploadedFilePath);
                try
                {
                    //string[] kode = inicodingnya.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                    string[] kode = inicodingnya.Split('\n');

                    int jumlahBarisKode = kode.Length;                    
                    //bool multiLineComment = false; // untuk menangani komentar multi-baris
                   
                    string hasil = "Jumlah baris kode: " + jumlahBarisKode;

                    HMetric2_textBox.Text = hasil;
                }
                catch (FileNotFoundException ex)
                {
                    MessageBox.Show($"File not found: {ex.FileName}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Select a file first.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }
        //End line count (halstead)


        //Start comment count (halstead)
        private void HCmntCount_BtnRun_Click(object sender, EventArgs e)
        {
            // Membersihkan konten sebelum menampilkan hasilnya
            HMetric2_textBox.Clear();
            HMetric3_textBox.Clear();
            HMetric4_textBox.Clear();

            if (!string.IsNullOrEmpty(uploadedFilePath) && File.Exists(uploadedFilePath))
            {
                string inicodingnya = File.ReadAllText(uploadedFilePath);
                try
                {
                    //string[] kode = inicodingnya.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                    string[] kode = inicodingnya.Split('\n');

                    //int jumlahBarisKode = kode.Length;
                    //int jumlahBarisKosong = 0;
                    int jumlahBarisKomentar = 0;
                    bool multiLineComment = false; // untuk menangani komentar multi-baris
                    foreach (string line in kode)
                    {                            
                        if (line.Trim().StartsWith("//"))
                        {
                            // Baris komentar satu baris
                            jumlahBarisKomentar++;

                        }
                        else if (line.Trim().StartsWith("/*"))
                        {
                            // Baris komentar multi-baris dimulai
                            multiLineComment = true;
                            jumlahBarisKomentar++;
                        }
                        else if (line.Trim().EndsWith("*/"))
                        {
                            // Baris komentar multi-baris berakhir
                            jumlahBarisKomentar++;
                            multiLineComment = false;
                        }
                        else if (multiLineComment)
                        {
                            // Baris dalam komentar multi-baris
                            jumlahBarisKomentar++;
                        }
                    }
                    string hasil = "Jumlah baris komentar: " + jumlahBarisKomentar + " baris";

                    HMetric2_textBox.Text = hasil;
                }
                catch (FileNotFoundException ex)
                {
                    MessageBox.Show($"File not found: {ex.FileName}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Select a file first.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }
        //End comment count (halstead)


        //Start blank line count (halstead)
        private void HBlankLn_BtnRun_Click(object sender, EventArgs e)
        {
            // Membersihkan konten sebelum menampilkan hasilnya
            HMetric2_textBox.Clear();
            HMetric3_textBox.Clear();
            HMetric4_textBox.Clear();

            if (!string.IsNullOrEmpty(uploadedFilePath) && File.Exists(uploadedFilePath))
            {
                string inicodingnya = File.ReadAllText(uploadedFilePath);
                try
                {
                    //string[] kode = inicodingnya.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                    string[] kode = inicodingnya.Split('\n');

                    //int jumlahBarisKode = kode.Length;
                    int jumlahBarisKosong = 0;
                    //int jumlahBarisKomentar = 0;
                    //bool multiLineComment = false; // untuk menangani komentar multi-baris
                    foreach (string line in kode)
                    {
                        if (string.IsNullOrWhiteSpace(line))
                        {
                            // Baris kosong
                            jumlahBarisKosong++;
                        }
                       
                    }
                    string hasil = "Jumlah baris kosong: " + jumlahBarisKosong + " baris";

                    HMetric2_textBox.Text = hasil;
                }
                catch (FileNotFoundException ex)
                {
                    MessageBox.Show($"File not found: {ex.FileName}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Select a file first.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }
        //End Blank line count (halstead)



        //biar export .csv ngak export file kosong
        private void HMetric2_textBox_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(HMetric2_textBox.Text) && !string.IsNullOrEmpty(HMetric3_textBox.Text) && !string.IsNullOrEmpty(HMetric4_textBox.Text))
            {
                CSV_Button.Enabled = true;
            }
        }
        private void HMetric3_textBox_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(HMetric2_textBox.Text) && !string.IsNullOrEmpty(HMetric3_textBox.Text) && !string.IsNullOrEmpty(HMetric4_textBox.Text))
            {
                CSV_Button.Enabled = true;
            }
        }
        private void HMetric4_textBox_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(HMetric2_textBox.Text) && !string.IsNullOrEmpty(HMetric3_textBox.Text) && !string.IsNullOrEmpty(HMetric4_textBox.Text))
            {
                CSV_Button.Enabled = true;
            }
        }

        private void tabPage4_Click(object sender, EventArgs e)
        {

        }
    }
    public class Module
    {
        public string Name { get; set; }
        public int Iv { get; set; }
        public List<Module> Children { get; set; }
        public List<string> ControlStructures { get; set; }
        public int So { get; private set; }

        public Module(string name)
        {
            Name = name;
            Iv = 0;
            Children = new List<Module>();
            ControlStructures = new List<string>();
        }

        public int CalculateSo()
        {
            So = Iv;
            foreach (var child in Children)
            {
                So += child.CalculateSo();
            }
            return So;
        }

    }
}