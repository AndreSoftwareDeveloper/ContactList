using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.ComponentModel.DataAnnotations;

namespace ContactList.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public ContactLoginForm LoginForm { get; set; }

        public String debug = "";
        public string loggedInUserEmail = "";
        public List<ContactInfo> listContacts = new List<ContactInfo>();
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public IActionResult OnPostLogin()
        {
            if (ModelState.IsValid)
            { 
                //TODO login data verification
                loggedInUserEmail = LoginForm.Email; //TODO security
                debug = "xxvvv";
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
                                while (reader.Read())
                                {
                                    int id = reader.GetInt32(0);
                                    string firstName = reader.GetString(1);
                                    string lastName = reader.GetString(2);
                                    string email = reader.GetString(3);
                                    string password = reader.GetString(4);
                                    string category = reader.GetString(5);
                                    string subCategory = reader.IsDBNull(6) ? null : reader.GetString(6);
                                    //int phone = reader.GetInt32(7); //tu przestaje działać                                
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
                //return RedirectToPage();
            }
            return Page();
        }

        public IActionResult OnPostDetails(int contactId)
        {
            return Page();
        }

        public void OnGet()
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
                            while (reader.Read())
                            {                                
                                int id = reader.GetInt32(0);
                                string firstName = reader.GetString(1);                                
                                string lastName = reader.GetString(2);                                
                                string email = reader.GetString(3);                                
                                string password = reader.GetString(4);                                
                                string category = reader.GetString(5);                                
                                string subCategory = reader.IsDBNull(6) ? null : reader.GetString(6);                             
                                //int phone = reader.GetInt32(7); //tu przestaje działać                                
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

        public void OnPost([FromQuery] string email, [FromQuery] string password)
        {
            debug = "qqq";
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
    }    
}
