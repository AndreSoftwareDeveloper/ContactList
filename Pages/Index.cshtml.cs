using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace ContactList.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public ContactLoginForm LoginForm { get; set; }

        [BindProperty]
        public ContactAddForm AddForm { get; set; }

        public string debug = "";
        public string loggedInUserEmail = "";
        public List<ContactInfo> listContacts = new List<ContactInfo>();
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public IActionResult OnPostLogin()
        {            
                loggedInUserEmail = LoginForm.Email;
                showContacts(); 
            return Page();
        }

        public IActionResult OnPostAdd(String email, String password, String firstName,
            String lastName, String category, String subCategory, int phone, String dateOfBirth)
        {
            String connectionString = "Data Source=.\\sqlexpress;Initial Catalog=clients;Integrated Security=True";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                String deleteQuery = "INSERT INTO Contacts (ContactID, FirstName, LastName, Email, Password, Category, Subcategory, Phone, BirthDate)" +
                    "VALUES (@Id, @FirstName, @LastName, @Email, @Password, @Category, @Subcategory, @Phone, @BirthDate)";
                using (SqlCommand addCommand = new SqlCommand(deleteQuery, connection))
                {
                    addCommand.Parameters.AddWithValue("@Id", ContactInfo.totalContacts+1);
                    addCommand.Parameters.Add("@FirstName", SqlDbType.NVarChar, 255).Value = firstName;
                    addCommand.Parameters.AddWithValue("@LastName", lastName);
                    addCommand.Parameters.AddWithValue("@Email", email);
                    addCommand.Parameters.AddWithValue("@Password", password);
                    addCommand.Parameters.AddWithValue("@Category", category);
                    addCommand.Parameters.AddWithValue("@Subcategory", subCategory);
                    addCommand.Parameters.AddWithValue("@Phone", phone);
                    addCommand.Parameters.AddWithValue("@BirthDate", dateOfBirth);
                    int rowsAffected = addCommand.ExecuteNonQuery();
                }
            }
            showContacts();
            return Page();
        }

        public IActionResult OnPostDelete(int contactId)
        {
            String connectionString = "Data Source=.\\sqlexpress;Initial Catalog=clients;Integrated Security=True";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                String deleteQuery = "DELETE FROM Contacts WHERE ContactID = @Id";
                using (SqlCommand deleteCommand = new SqlCommand(deleteQuery, connection))
                {
                    deleteCommand.Parameters.AddWithValue("@Id", contactId);
                    int rowsAffected = deleteCommand.ExecuteNonQuery();
                }
            }
            showContacts();
            return Page();
        }

        public IActionResult OnPostDetails(int contactId)
        {
            return Page();
        }

        public void OnGet()
        {
            showContacts();            
        }

        private void showContacts()
        {
            try
            {
                String connectionString = "Data Source=.\\sqlexpress;Initial Catalog=clients;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String query = "SELECT * FROM Contacts";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            ContactInfo.totalContacts = 0;
                            while (reader.Read())
                            {
                                int id = reader.GetInt32(0);
                                string firstName = reader.GetString(1);
                                string lastName = reader.GetString(2);
                                string email = reader.GetString(3);
                                string password = reader.GetString(4);
                                string category = reader.GetString(5);
                                string subCategory = reader.IsDBNull(6) ? null : reader.GetString(6);
                                //int phone = reader.GetInt32(7);                             
                                string birthDate = reader.GetDateTime(8).ToString("yyyy-MM-dd");

                                ContactInfo contactInfo = new ContactInfo(firstName, lastName, email, password, category, subCategory, 111222333, birthDate);
                                listContacts.Add(contactInfo);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
            }            
        }

        public class ContactInfo
        {
            public static int totalContacts = 0;

            public int id;
            public String firstName;
            public String lastName;
            public String email;
            public String password;
            public String category;
            public String subCategory;
            public int phone;
            public String birthDate;

            public ContactInfo(String firstName, String lastName, String email, String password, String category, String subCategory, int phone, String birthDate)
            {
                this.id = ++totalContacts;
                this.firstName = firstName;
                this.lastName = lastName;
                this.email = email;
                this.password = password;
                this.category = category;
                this.subCategory = subCategory;
                this.phone = phone;
                this.birthDate = birthDate;
            }
        }

        public class ContactLoginForm
        {
            [EmailAddress(ErrorMessage = "Invalid Email Address")]
            [Required(ErrorMessage = "Email is required")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Password is required")]
            public string Password { get; set; }
        }

        public class ContactAddForm
        {
            public string Email { get; set; }
            public string Password { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Category { get; set; }
            public string Subcategory { get; set; }
            public string DateOfBirth { get; set; }
            public int Phone { get; set; }
        }
    }    
}
