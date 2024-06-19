using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using backend_app.Models;

namespace backend_app.Controllers
{
    [RoutePrefix("api/Test")]
    public class TestController : ApiController
    {
        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["conn"].ConnectionString);
        SqlCommand cmd = null;  
        SqlDataAdapter da= null;

        [HttpPost]
        [Route("Registration")]
        public string Registration(Employee employee)
        {
            string msg = string.Empty;
            try
            {
                cmd = new SqlCommand("Emp_Registartion", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Name", employee.Name);
                cmd.Parameters.AddWithValue("@PhoneNo", employee.PhoneNo);
                cmd.Parameters.AddWithValue("@Address", employee.Address);
                cmd.Parameters.AddWithValue("@Email", employee.Email);
                cmd.Parameters.AddWithValue("@Password", employee.Password);

                conn.Open();
                int i = cmd.ExecuteNonQuery();
                conn.Close();
                if (i > 0)
                {
                    msg = "Data inserted.";
                }
                else
                {
                    msg = "Error.";
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }

           
            return msg;
        }

        [HttpPost]
        [Route("Login")]
        public string Login(Employee employee)
        {
            string msg = string.Empty;
            try
            {
                da = new SqlDataAdapter("Emp_Login", conn);
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                da.SelectCommand.Parameters.AddWithValue("@Email", employee.Email);
                da.SelectCommand.Parameters.AddWithValue("@Password", employee.Password);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if(dt.Rows.Count > 0)
                {
                    msg = "User is valid";
                }
                else
                {
                    msg = "User is Invalid";
                }

            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }


            return msg;

        }

        [HttpPost]
        [Route("TodoInsert")]
        public string TodoInsert(Todo todo)
        {
            string msg = string.Empty;

            try
            {
                int employeeId = GetEmployeeIdByEmail(todo.Email);

                if (employeeId == 0)
                {
                    return "Employee not found.";
                }

                
                {
                    SqlCommand cmd = new SqlCommand("Todo_Insert", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Title", todo.Title);
                    cmd.Parameters.AddWithValue("@Category", todo.Category);
                    cmd.Parameters.AddWithValue("@DueDate", todo.DueDate);
                    cmd.Parameters.AddWithValue("@Estimate", todo.Estimate);
                    cmd.Parameters.AddWithValue("@Importance", todo.Importance);
                    cmd.Parameters.AddWithValue("@Email", todo.Email);

                    conn.Open();
                    int i = cmd.ExecuteNonQuery();
                    conn.Close();

                    if (i > 0)
                    {
                        msg = "Todo inserted.";
                    }
                    else
                    {
                        msg = "Error inserting todo.";
                    }
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }

            return msg;
        }

        private int GetEmployeeIdByEmail(string email)
        {
            int employeeId = 0;

            try
            {
                
                {
                    SqlCommand cmd = new SqlCommand("SELECT Id FROM Employee WHERE Email = @Email", conn);
                    cmd.Parameters.AddWithValue("@Email", email);

                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    conn.Close();

                    if (result != null && result != DBNull.Value)
                    {
                        employeeId = Convert.ToInt32(result);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return employeeId;
        }

        [HttpGet]
        [Route("GetTodosByEmail")]
        public IHttpActionResult GetTodosByEmail([FromUri] string email)
        {
            List<Todo> todos = new List<Todo>();

            try
            {
                using (SqlCommand cmd = new SqlCommand("Todo_GetDataByEmail", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Email", email);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Todo todo = new Todo
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Title = reader.GetString(reader.GetOrdinal("Title")),
                                Status = reader.GetString(reader.GetOrdinal("Status")),
                                Category = reader.GetString(reader.GetOrdinal("Category")),
                                DueDate = reader.GetDateTime(reader.GetOrdinal("DueDate")),
                                Estimate = reader.GetString(reader.GetOrdinal("Estimate")),
                                Importance = reader.GetString(reader.GetOrdinal("Importance")),
                                Email = email
                            };
                            todos.Add(todo);
                        }
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(todos);
        }

        [HttpPut]
        [Route("UpdateTodoTitle")]
        public IHttpActionResult UpdateTodosTitle([FromUri] int Id, [FromUri] string Title)
        {
            if (string.IsNullOrEmpty(Title))
            {
                return BadRequest("Title cannot be empty.");
            }

            string msg = string.Empty;

            try
            {
                {
                    using (SqlCommand cmd = new SqlCommand("Todo_UpdateTitle", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Id", Id);
                        cmd.Parameters.AddWithValue("@Title", Title);

                        conn.Open();
                        int i = cmd.ExecuteNonQuery();
                        conn.Close();

                        if (i > 0)
                        {
                            msg = "Title Changed.";
                        }
                        else
                        {
                            msg = "Error while changing Title.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }

            return Ok(msg);
        }

        [HttpPut]
        [Route("UpdateTodoStatus")]
        public IHttpActionResult UpdateTodoStatus([FromUri] int Id, [FromUri] string Status)
        {
            if (string.IsNullOrEmpty(Status))
            {
                return BadRequest("Status cannot be empty.");
            }

            string msg = string.Empty;

            try
            {
                {
                    using (SqlCommand cmd = new SqlCommand("Todo_ChangeStatus", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Id", Id);
                        cmd.Parameters.AddWithValue("@Status", Status);

                        conn.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();
                        conn.Close();

                        if (rowsAffected > 0)
                        {
                            msg = "Status updated successfully.";
                        }
                        else
                        {
                            msg = "No records updated. Check if the provided Id exists.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                msg = $"Error updating status: {ex.Message}";
            }
            return Ok(msg);
        }




    }
}
