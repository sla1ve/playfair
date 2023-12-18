using System.Text;
using System.Text.RegularExpressions;

namespace Playfair
{
    public partial class Playfair : Form
    {
        public Playfair()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private char[,] GeneratePlayfairMatrix(List<char> characters)
        {
            // Tạo một ma trận 5x5 để sử dụng trong thuật toán mã hóa Playfair
            char[,] matrix = new char[5, 5];
            int index = 0;

            for (int row = 0; row < 5; row++)
            {
                for (int col = 0; col < 5; col++)
                {
                    // Loại bỏ ký tự 'J' nếu có
                    while (characters[index] == 'J')
                    {
                        index++;
                    }

                    matrix[row, col] = characters[index++];
                }
            }

            return matrix;
        }

        private void DisplayMatrix(char[,] matrix)
        {
            // Hiển thị ma trận trong các TextBox từ TextBox1 đến TextBox25
            int index = 0;

            for (int row = 0; row < 5; row++)
            {
                for (int col = 0; col < 5; col++)
                {

                    TextBox textBox = Controls.Find($"textBox{index + 1}", true).FirstOrDefault() as TextBox;

                    if (textBox != null)
                    {
                        // Hiển thị giá trị từ ma trận vào TextBox
                        textBox.Text = matrix[row, col].ToString();
                        index++;
                    }
                }
            }
        }

        private List<char> ProcessKeyword(string keyword)
        {

            // Loại bỏ ký tự 'J' nếu có
            keyword = keyword.Replace("J", "");

            // Loại bỏ ký tự khoảng trắng
            keyword = keyword.Replace(" ", "");

            // Loại bỏ các ký tự trùng lặp trong từ khóa
            string uniqueChars = new string(keyword.Distinct().ToArray());

            // Tạo danh sách ký tự từ từ khóa và bổ sung các ký tự còn lại của bảng chữ cái
            List<char> characters = uniqueChars.ToList();
            characters.AddRange(Enumerable.Range('A', 26).Select(c => (char)c).Except(uniqueChars));

            return characters;
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            // Lấy từ khóa từ txtKeyword
            string keyword = txtKeyword.Text.ToUpper();

            // Xử lý từ khóa để tạo danh sách ký tự cho ma trận Playfair
            List<char> characters = ProcessKeyword(keyword);

            // Tạo ma trận Playfair từ danh sách ký tự đã xử lý
            char[,] playfairMatrix = GeneratePlayfairMatrix(characters);

            // Hiển thị ma trận trong các TextBox từ TextBox1 đến TextBox25
            DisplayMatrix(playfairMatrix);
        }

        private char[,] GetPlayfairMatrixFromTextBoxes()
        {
            // Tạo ma trận 5x5 từ TextBoxes
            char[,] matrix = new char[5, 5];
            int index = 0;

            for (int row = 0; row < 5; row++)
            {
                for (int col = 0; col < 5; col++)
                {
                    // Tìm TextBox theo tên
                    TextBox textBox = Controls.Find($"textBox{index + 1}", true).FirstOrDefault() as TextBox;

                    if (textBox != null)
                    {
                        // Lấy giá trị từ TextBox và đặt vào ma trận
                        matrix[row, col] = textBox.Text[0];
                        index++;
                    }
                }
            }

            return matrix;
        }

        private Tuple<int, int> FindCharPosition(char[,] playfairMatrix, char target)
        {
            for (int row = 0; row < 5; row++)
            {
                for (int col = 0; col < 5; col++)
                {
                    if (playfairMatrix[row, col] == target)
                    {
                        return Tuple.Create(row, col);
                    }
                }
            }

            // Trả về giá trị mặc định nếu không tìm thấy
            return Tuple.Create(-1, -1);
        }

        private string ProcessInput(string input)
        {
            // Chuẩn hóa văn bản đầu vào
            input = input.ToUpper().Replace("J", "I");
            input = Regex.Replace(input, "[^A-Z]", "");

            // Thêm 'X' giữa các cặp ký tự giống nhau hoặc giữa ký tự cuối cùng nếu số ký tự là lẻ
            for (int i = 0; i < input.Length - 1; i += 2)
            {
                if (input[i] == input[i + 1])
                {
                    input = input.Insert(i + 1, "X");
                }
            }

            if (input.Length % 2 != 0)
            {
                input += "X";
            }

            return input;
        }

        private string PlayfairEncrypt(string plaintext, char[,] playfairMatrix)
        {
            // Chuẩn hóa văn bản đầu vào
            plaintext = ProcessInput(plaintext);

            StringBuilder ciphertext = new StringBuilder();

            for (int i = 0; i < plaintext.Length - 1; i += 2)
            {
                char char1 = plaintext[i];
                char char2 = plaintext[i + 1];

                // Tìm vị trí của các ký tự trong ma trận Playfair
                Tuple<int, int> pos1 = FindCharPosition(playfairMatrix, char1);
                Tuple<int, int> pos2 = FindCharPosition(playfairMatrix, char2);

                if (pos1.Item1 == pos2.Item1) // Cùng hàng
                {
                    ciphertext.Append(playfairMatrix[pos1.Item1, (pos1.Item2 + 1) % 5]);
                    ciphertext.Append(playfairMatrix[pos2.Item1, (pos2.Item2 + 1) % 5]);
                }
                else if (pos1.Item2 == pos2.Item2) // Cùng cột
                {
                    ciphertext.Append(playfairMatrix[(pos1.Item1 + 1) % 5, pos1.Item2]);
                    ciphertext.Append(playfairMatrix[(pos2.Item1 + 1) % 5, pos2.Item2]);
                }
                else // Tạo hình chữ nhật
                {
                    ciphertext.Append(playfairMatrix[pos1.Item1, pos2.Item2]);
                    ciphertext.Append(playfairMatrix[pos2.Item1, pos1.Item2]);
                }
            }

            return ciphertext.ToString();
        }

        private string PlayfairDecrypt(string ciphertext, char[,] playfairMatrix)
        {
            // Chuẩn hóa văn bản đầu vào
            ciphertext = ProcessInput(ciphertext);

            StringBuilder plaintext = new StringBuilder();

            for (int i = 0; i < ciphertext.Length - 1; i += 2)
            {
                char char1 = ciphertext[i];
                char char2 = ciphertext[i + 1];

                // Tìm vị trí của các ký tự trong ma trận Playfair
                Tuple<int, int> pos1 = FindCharPosition(playfairMatrix, char1);
                Tuple<int, int> pos2 = FindCharPosition(playfairMatrix, char2);

                if (pos1.Item1 == pos2.Item1) // Cùng hàng
                {
                    plaintext.Append(playfairMatrix[pos1.Item1, (pos1.Item2 - 1 + 5) % 5]);
                    plaintext.Append(playfairMatrix[pos2.Item1, (pos2.Item2 - 1 + 5) % 5]);
                }
                else if (pos1.Item2 == pos2.Item2) // Cùng cột
                {
                    plaintext.Append(playfairMatrix[(pos1.Item1 - 1 + 5) % 5, pos1.Item2]);
                    plaintext.Append(playfairMatrix[(pos2.Item1 - 1 + 5) % 5, pos2.Item2]);
                }
                else // Tạo hình chữ nhật
                {
                    plaintext.Append(playfairMatrix[pos1.Item1, pos2.Item2]);
                    plaintext.Append(playfairMatrix[pos2.Item1, pos1.Item2]);
                }
            }

            return plaintext.ToString();
        }

        private bool AreTextBoxesFilled()
        {
            // Kiểm tra xem tất cả TextBox từ textBox1 đến textBox25 có giá trị hay không
            for (int i = 1; i <= 25; i++)
            {
                TextBox textBox = Controls.Find($"textBox{i}", true).FirstOrDefault() as TextBox;

                if (textBox == null || string.IsNullOrWhiteSpace(textBox.Text))
                {
                    return false; // Có ít nhất một TextBox chưa được điền giá trị
                }
            }

            return true; // Tất cả TextBox đã được điền giá trị
        }

        private void btnEncrypt_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem ma trận Playfair đã được tạo chưa
            if (!AreTextBoxesFilled())
            {
                MessageBox.Show("Hãy ấn vào nút Generate để tạo ma trận.", "Ma trận chưa được tạo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Stop further execution
            }

            // Lấy văn bản gốc từ txtPlaintext
            string plaintext = txtPlaintext.Text.ToUpper();

            // Lấy ma trận Playfair từ TextBoxes
            char[,] playfairMatrix = GetPlayfairMatrixFromTextBoxes();

            // Mã hóa văn bản
            string ciphertext = PlayfairEncrypt(plaintext, playfairMatrix);

            // Hiển thị văn bản đã mã hóa trong txtCiphertext
            txtCiphertext.Text = ciphertext;
        }

        private void btnDecrypt_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem ma trận Playfair đã được tạo chưa
            if (!AreTextBoxesFilled())
            {
                MessageBox.Show("Hãy ấn vào nút Generate để tạo ma trận.", "Ma trận chưa được tạo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Stop further execution
            }

            // Lấy văn bản gốc từ txtCiphertext
            string ciphertext = txtCiphertext.Text.ToUpper();

            // Lấy ma trận Playfair từ TextBoxes
            char[,] playfairMatrix = GetPlayfairMatrixFromTextBoxes();

            // Giải mã văn bản
            string plaintext = PlayfairDecrypt(ciphertext, playfairMatrix);

            // Hiển thị văn bản đã giải mã trong txtCiphertext
            txtPlaintext.Text = plaintext;
        }
    }
}