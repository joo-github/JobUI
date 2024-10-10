using System;
using JobBL;
using JobModel;

// Add necessary using directives for MailKit and MimeKit
using MailKit.Net.Smtp;
using MimeKit;

namespace JobUI
{
    class Program
    {
        static void Main(string[] args)
        {
            BL bl = new BL();

            Console.Write("Enter username: ");
            string username = Console.ReadLine();

            Console.Write("Enter password: "); 
            string password = Console.ReadLine();
            //DL.Connect();

            if (bl.ValidateUser(username, password))
            {
                while (true)
                {
                    Console.WriteLine("\nOptions:");
                    Console.WriteLine("1. View Jobs");
                    Console.WriteLine("2. Add New Job");
                    Console.WriteLine("3. Update Job Details");
                    Console.WriteLine("4. Delete Job");
                    Console.Write("Enter your choice: ");
                    string choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            ViewJobs(bl);
                            break;

                        case "2":
                            AddJob(bl);
                            break;

                        case "3":
                            UpdateJob(bl);
                            break;

                        case "4":
                            DeleteJob(bl);
                            break;

                        default:
                            Console.WriteLine("Invalid choice. Please try again.");
                            break;
                    }

                    Console.Write("\nDo you want to perform another operation? (yes/no): ");
                    string continueChoice = Console.ReadLine();
                    if (continueChoice.Equals("no", StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
            else
            {
                Console.WriteLine("Invalid username or password.");
            }
        }

        // Helper method to send emails using Mailtrap
        static void SendEmail(string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("FromMyNotes", "do-not-reply@frommynotes.com"));
            message.To.Add(new MailboxAddress("User", "user@example.com"));
            message.Subject = subject;

            message.Body = new TextPart("html")
            {
                Text = body
            };

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                try
                {
                    // Connect to Mailtrap SMTP server
                    client.Connect("sandbox.smtp.mailtrap.io", 2525, MailKit.Security.SecureSocketOptions.StartTls);

                    // Authenticate with Mailtrap credentials
                    client.Authenticate("a9016ec29cbf7a", "d29d49b3ebe3cb");

                    // Send the email
                    client.Send(message);
                    Console.WriteLine("Notification email sent successfully through Mailtrap.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error sending email: {ex.Message}");
                }
                finally
                {
                    client.Disconnect(true);
                }
            }
        }

        // Method to view all jobs
        static void ViewJobs(BL bl)
        {
            var jobs = bl.GetAllJobs();
            Console.WriteLine("\nJob Lists:");
            foreach (var job in jobs)
            {
                Console.WriteLine($"Job Title: {job.JobTitle}");
                Console.WriteLine($"Description: {job.JobDescription}");
                Console.WriteLine($"Company: {job.Company}");
                Console.WriteLine($"Salary: {job.Salary}");
                Console.WriteLine($"Location: {job.Location}");
                Console.WriteLine();
            }

            // Send email notification for viewing jobs
            string subject = "Job Listings Viewed";
            string body = "<h1>Job Listings Viewed</h1><p>You have viewed the list of available jobs.</p>";
            SendEmail(subject, body);
        }

        // Method to add a new job
        static void AddJob(BL bl)
        {
            Console.Write("Job Title: ");
            string newTitle = Console.ReadLine();
            Console.Write("Job Description: ");
            string newDescription = Console.ReadLine();
            Console.Write("Company: ");
            string newCompany = Console.ReadLine();
            Console.Write("Salary: ");
            string newSalary = Console.ReadLine();
            Console.Write("Location: ");
            string newLocation = Console.ReadLine();

            Console.Write("Type DONE to Save: ");
            string doneInput = Console.ReadLine();

            if (doneInput.Equals("DONE", StringComparison.OrdinalIgnoreCase))
            {
                Job newJob = new Job
                {
                    JobTitle = newTitle,
                    JobDescription = newDescription,
                    Company = newCompany,
                    Salary = newSalary,
                    Location = newLocation
                };

                bl.AddNewJob(newJob);
                Console.WriteLine("New job added successfully.");

                // Send email notification for adding a new job
                string subject = "New Job Added";
                string body = $"<h1>New Job Added</h1><p>A new job titled <strong>{newTitle}</strong> has been added.</p>";
                SendEmail(subject, body);
            }
            else
            {
                Console.WriteLine("Process Cancelled. No changes made.");
            }
        }

        // Method to update an existing job
        static void UpdateJob(BL bl)
        {
            Console.Write("Enter Job Title to update details: ");
            string updateTitle = Console.ReadLine();

            var jobToUpdate = bl.GetJobByTitle(updateTitle);
            if (jobToUpdate != null)
            {
                Console.Write($"Enter new Job Description (current: {jobToUpdate.JobDescription}): ");
                string updateDescription = Console.ReadLine();

                Console.Write($"Enter new Company (current: {jobToUpdate.Company}): ");
                string updateCompany = Console.ReadLine();

                Console.Write($"Enter new Salary (current: {jobToUpdate.Salary}): ");
                string updateSalary = Console.ReadLine();

                Console.Write($"Enter new Location (current: {jobToUpdate.Location}): ");
                string updateLocation = Console.ReadLine();

                Console.Write("Enter DONE to save changes: ");
                string doneUpdate = Console.ReadLine();

                if (doneUpdate.Equals("DONE", StringComparison.OrdinalIgnoreCase))
                {
                    jobToUpdate.JobDescription = updateDescription;
                    jobToUpdate.Company = updateCompany;
                    jobToUpdate.Salary = updateSalary;
                    jobToUpdate.Location = updateLocation;

                    bl.UpdateJob(jobToUpdate);
                    Console.WriteLine("Job details updated successfully.");

                    // Send email notification for updating a job
                    string subject = "Job Updated";
                    string body = $"<h1>Job Updated</h1><p>The job titled <strong>{updateTitle}</strong> has been updated.</p>";
                    SendEmail(subject, body);
                }
                else
                {
                    Console.WriteLine("Process cancelled. No changes made.");
                }
            }
            else
            {
                Console.WriteLine("Job not found.");
            }
        }

        // Method to delete a job
        static void DeleteJob(BL bl)
        {
            Console.Write("Enter Job Title to delete: ");
            string deleteTitle = Console.ReadLine();

            var jobToDelete = bl.GetJobByTitle(deleteTitle);
            if (jobToDelete != null)
            {
                bl.DeleteJob(deleteTitle);
                Console.WriteLine("Job deleted successfully.");

                // Send email notification for deleting a job
                string subject = "Job Deleted";
                string body = $"<h1>Job Deleted</h1><p>The job titled <strong>{deleteTitle}</strong> has been deleted.</p>";
                SendEmail(subject, body);
            }
            else
            {
                Console.WriteLine("Job not found.");
            }
        }
    }
}
