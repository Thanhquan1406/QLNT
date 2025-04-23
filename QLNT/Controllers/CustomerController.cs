using Microsoft.AspNetCore.Mvc;
using QLNT.Models;
using QLNT.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Linq;

namespace QLNT.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CustomerController(ICustomerRepository customerRepository, IWebHostEnvironment webHostEnvironment)
        {
            _customerRepository = customerRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        private async Task<string> SaveFile(IFormFile file, string subFolder)
        {
            if (file == null || file.Length == 0)
            {
                Console.WriteLine("No file to save");
                return null;
            }

            // Kiểm tra định dạng file
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
            {
                Console.WriteLine($"Invalid file extension: {extension}");
                ModelState.AddModelError("", "Chỉ chấp nhận file ảnh có định dạng JPG, JPEG, PNG hoặc GIF");
                return null;
            }

            // Kiểm tra kích thước file (tối đa 5MB)
            const int maxFileSize = 5 * 1024 * 1024; // 5MB
            if (file.Length > maxFileSize)
            {
                Console.WriteLine($"File too large: {file.Length} bytes");
                ModelState.AddModelError("", "Kích thước file không được vượt quá 5MB");
                return null;
            }

            try
            {
                Console.WriteLine($"WebRootPath: {_webHostEnvironment.WebRootPath}");
                Console.WriteLine($"File name: {file.FileName}");
                Console.WriteLine($"File length: {file.Length}");
                Console.WriteLine($"File content type: {file.ContentType}");
                
                // Tạo thư mục nếu chưa tồn tại
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Images", "customers", subFolder);
                Console.WriteLine($"Upload folder path: {uploadsFolder}");
                
                if (!Directory.Exists(uploadsFolder))
                {
                    try
                    {
                        Console.WriteLine($"Creating directory: {uploadsFolder}");
                        Directory.CreateDirectory(uploadsFolder);
                        Console.WriteLine($"Directory created successfully");
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        Console.WriteLine($"Access denied when creating directory: {ex.Message}");
                        ModelState.AddModelError("", "Không có quyền tạo thư mục. Vui lòng kiểm tra quyền truy cập.");
                        return null;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error creating directory: {ex.Message}");
                        ModelState.AddModelError("", $"Lỗi khi tạo thư mục: {ex.Message}");
                        return null;
                    }
                }

                // Kiểm tra quyền ghi vào thư mục
                try
                {
                    var testFile = Path.Combine(uploadsFolder, "test.tmp");
                    Console.WriteLine($"Testing write permission with file: {testFile}");
                    System.IO.File.WriteAllText(testFile, "test");
                    System.IO.File.Delete(testFile);
                    Console.WriteLine("Write permission test successful");
                }
                catch (UnauthorizedAccessException ex)
                {
                    Console.WriteLine($"Access denied when testing write permission: {ex.Message}");
                    ModelState.AddModelError("", "Không có quyền ghi vào thư mục. Vui lòng kiểm tra quyền truy cập.");
                    return null;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error testing write permission: {ex.Message}");
                    ModelState.AddModelError("", $"Lỗi khi kiểm tra quyền ghi: {ex.Message}");
                    return null;
                }

                // Tạo tên file duy nhất
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                Console.WriteLine($"File will be saved to: {filePath}");

                // Lưu file
                try
                {
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        Console.WriteLine("Copying file to stream...");
                        await file.CopyToAsync(fileStream);
                        Console.WriteLine("File copied successfully");
                    }

                    // Kiểm tra xem file đã được lưu thành công chưa
                    if (System.IO.File.Exists(filePath))
                    {
                        Console.WriteLine($"File exists after saving: {filePath}");
                        Console.WriteLine($"File size after saving: {new FileInfo(filePath).Length} bytes");
                    }
                    else
                    {
                        Console.WriteLine($"File does not exist after saving: {filePath}");
                        ModelState.AddModelError("", "Không thể lưu file. Vui lòng thử lại.");
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error saving file: {ex.Message}");
                    Console.WriteLine($"Stack trace: {ex.StackTrace}");
                    ModelState.AddModelError("", $"Lỗi khi lưu file: {ex.Message}");
                    return null;
                }

                // Trả về đường dẫn tương đối
                string relativePath = $"/Images/customers/{subFolder}/{uniqueFileName}";
                Console.WriteLine($"Returning relative path: {relativePath}");
                return relativePath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SaveFile: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                ModelState.AddModelError("", $"Lỗi khi lưu file: {ex.Message}");
                return null;
            }
        }

        // GET: Customer
        public async Task<IActionResult> Index()
        {
            var customers = await _customerRepository.GetAllAsync();
            return View(customers);
        }

        // GET: Customer/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _customerRepository.GetByIdAsync(id.Value);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // GET: Customer/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Customer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CustomerViewModel customerViewModel)
        {
            try
            {
                Console.WriteLine("Starting to create customer...");
                
                // Xử lý upload file trước
                if (customerViewModel.RepresentativeImageFile != null && customerViewModel.RepresentativeImageFile.Length > 0)
                {
                    var imagePath = await SaveFile(customerViewModel.RepresentativeImageFile, "representative");
                    if (imagePath != null)
                    {
                        customerViewModel.RepresentativeImage = imagePath;
                        ModelState.Remove("RepresentativeImage"); // Xóa validation cho RepresentativeImage
                    }
                }

                if (customerViewModel.FrontIdentityCardFile != null && customerViewModel.FrontIdentityCardFile.Length > 0)
                {
                    var imagePath = await SaveFile(customerViewModel.FrontIdentityCardFile, "identity_card");
                    if (imagePath != null)
                    {
                        customerViewModel.FrontIdentityCard = imagePath;
                        ModelState.Remove("FrontIdentityCard");
                    }
                }

                if (customerViewModel.BackIdentityCardFile != null && customerViewModel.BackIdentityCardFile.Length > 0)
                {
                    var imagePath = await SaveFile(customerViewModel.BackIdentityCardFile, "identity_card");
                    if (imagePath != null)
                    {
                        customerViewModel.BackIdentityCard = imagePath;
                        ModelState.Remove("BackIdentityCard");
                    }
                }

                if (customerViewModel.PortraitFile != null && customerViewModel.PortraitFile.Length > 0)
                {
                    var imagePath = await SaveFile(customerViewModel.PortraitFile, "portrait");
                    if (imagePath != null)
                    {
                        customerViewModel.Portrait = imagePath;
                        ModelState.Remove("Portrait");
                    }
                }

                // Kiểm tra ModelState sau khi đã xử lý file
                if (!ModelState.IsValid)
                {
                    Console.WriteLine("\n=== Lỗi validation ===");
                    foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        Console.WriteLine($"Validation error: {error.ErrorMessage}");
                    }
                    return View(customerViewModel);
                }

                var customer = new Customer
                {
                    FullName = customerViewModel.FullName,
                    PhoneNumber = customerViewModel.PhoneNumber,
                    Email = customerViewModel.Email,
                    DateOfBirth = customerViewModel.DateOfBirth,
                    Gender = customerViewModel.Gender,
                    IdentityCard = customerViewModel.IdentityCard,
                    IdentityCardIssueDate = customerViewModel.IdentityCardIssueDate,
                    IdentityCardIssuePlace = customerViewModel.IdentityCardIssuePlace,
                    Province = customerViewModel.Province,
                    District = customerViewModel.District,
                    Ward = customerViewModel.Ward,
                    Address = customerViewModel.Address,
                    BankAccount = customerViewModel.BankAccount,
                    BankName = customerViewModel.BankName,
                    Occupation = customerViewModel.Occupation,
                    Workplace = customerViewModel.Workplace,
                    EmergencyContact = customerViewModel.EmergencyContact,
                    EmergencyPhone = customerViewModel.EmergencyPhone,
                    Consultant = customerViewModel.Consultant,
                    ConsultantPhone = customerViewModel.ConsultantPhone,
                    AccessCode = customerViewModel.AccessCode,
                    IsForeigner = customerViewModel.IsForeigner,
                    Note = customerViewModel.Note,
                    RepresentativeImage = customerViewModel.RepresentativeImage,
                    FrontIdentityCard = customerViewModel.FrontIdentityCard,
                    BackIdentityCard = customerViewModel.BackIdentityCard,
                    Portrait = customerViewModel.Portrait
                };

                Console.WriteLine("\n=== Kiểm tra giá trị trước khi lưu ===");
                Console.WriteLine($"RepresentativeImage: {customer.RepresentativeImage ?? "null"}");
                Console.WriteLine($"FrontIdentityCard: {customer.FrontIdentityCard ?? "null"}");
                Console.WriteLine($"BackIdentityCard: {customer.BackIdentityCard ?? "null"}");
                Console.WriteLine($"Portrait: {customer.Portrait ?? "null"}");

                await _customerRepository.AddAsync(customer);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating customer: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                ModelState.AddModelError("", $"Lỗi khi tạo khách hàng: {ex.Message}");
                return View(customerViewModel);
            }
        }

        // GET: Customer/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _customerRepository.GetByIdAsync(id.Value);
            if (customer == null)
            {
                return NotFound();
            }

            var customerViewModel = new CustomerViewModel
            {
                Id = customer.Id,
                FullName = customer.FullName,
                PhoneNumber = customer.PhoneNumber,
                Email = customer.Email,
                DateOfBirth = customer.DateOfBirth,
                Gender = customer.Gender,
                IdentityCard = customer.IdentityCard,
                IdentityCardIssueDate = customer.IdentityCardIssueDate,
                IdentityCardIssuePlace = customer.IdentityCardIssuePlace,
                Province = customer.Province,
                District = customer.District,
                Ward = customer.Ward,
                Address = customer.Address,
                BankAccount = customer.BankAccount,
                BankName = customer.BankName,
                Occupation = customer.Occupation,
                Workplace = customer.Workplace,
                EmergencyContact = customer.EmergencyContact,
                EmergencyPhone = customer.EmergencyPhone,
                Consultant = customer.Consultant,
                ConsultantPhone = customer.ConsultantPhone,
                AccessCode = customer.AccessCode,
                IsForeigner = customer.IsForeigner,
                Note = customer.Note,
                RepresentativeImage = customer.RepresentativeImage,
                FrontIdentityCard = customer.FrontIdentityCard,
                BackIdentityCard = customer.BackIdentityCard,
                Portrait = customer.Portrait
            };

            return View(customerViewModel);
        }

        // POST: Customer/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,PhoneNumber,Email,DateOfBirth,Gender,IdentityCard,IdentityCardIssueDate,IdentityCardIssuePlace,Province,District,Ward,Address,BankAccount,BankName,Occupation,Workplace,EmergencyContact,EmergencyPhone,Consultant,ConsultantPhone,AccessCode,IsForeigner,Note,RepresentativeImage,FrontIdentityCard,BackIdentityCard,Portrait,RepresentativeImageFile,FrontIdentityCardFile,BackIdentityCardFile,PortraitFile")] CustomerViewModel customerViewModel)
        {
            if (id != customerViewModel.Id)
            {
                return NotFound();
            }

            try
            {
                Console.WriteLine("Starting Edit action...");
                var customer = await _customerRepository.GetByIdAsync(id);
                if (customer == null)
                {
                    return NotFound();
                }

                // Xử lý upload file mới và cập nhật đường dẫn
                if (customerViewModel.RepresentativeImageFile != null && customerViewModel.RepresentativeImageFile.Length > 0)
                {
                    var newPath = await SaveFile(customerViewModel.RepresentativeImageFile, "representative");
                    if (newPath != null)
                    {
                        // Xóa file cũ nếu tồn tại
                        if (!string.IsNullOrEmpty(customer.RepresentativeImage))
                        {
                            var oldPath = Path.Combine(_webHostEnvironment.WebRootPath, customer.RepresentativeImage.TrimStart('/'));
                            if (System.IO.File.Exists(oldPath))
                            {
                                System.IO.File.Delete(oldPath);
                            }
                        }
                        customer.RepresentativeImage = newPath;
                    }
                }

                if (customerViewModel.FrontIdentityCardFile != null && customerViewModel.FrontIdentityCardFile.Length > 0)
                {
                    var newPath = await SaveFile(customerViewModel.FrontIdentityCardFile, "identity_card");
                    if (newPath != null)
                    {
                        if (!string.IsNullOrEmpty(customer.FrontIdentityCard))
                        {
                            var oldPath = Path.Combine(_webHostEnvironment.WebRootPath, customer.FrontIdentityCard.TrimStart('/'));
                            if (System.IO.File.Exists(oldPath))
                            {
                                System.IO.File.Delete(oldPath);
                            }
                        }
                        customer.FrontIdentityCard = newPath;
                    }
                }

                if (customerViewModel.BackIdentityCardFile != null && customerViewModel.BackIdentityCardFile.Length > 0)
                {
                    var newPath = await SaveFile(customerViewModel.BackIdentityCardFile, "identity_card");
                    if (newPath != null)
                    {
                        if (!string.IsNullOrEmpty(customer.BackIdentityCard))
                        {
                            var oldPath = Path.Combine(_webHostEnvironment.WebRootPath, customer.BackIdentityCard.TrimStart('/'));
                            if (System.IO.File.Exists(oldPath))
                            {
                                System.IO.File.Delete(oldPath);
                            }
                        }
                        customer.BackIdentityCard = newPath;
                    }
                }

                if (customerViewModel.PortraitFile != null && customerViewModel.PortraitFile.Length > 0)
                {
                    var newPath = await SaveFile(customerViewModel.PortraitFile, "portrait");
                    if (newPath != null)
                    {
                        if (!string.IsNullOrEmpty(customer.Portrait))
                        {
                            var oldPath = Path.Combine(_webHostEnvironment.WebRootPath, customer.Portrait.TrimStart('/'));
                            if (System.IO.File.Exists(oldPath))
                            {
                                System.IO.File.Delete(oldPath);
                            }
                        }
                        customer.Portrait = newPath;
                    }
                }

                // Cập nhật thông tin cơ bản
                customer.FullName = customerViewModel.FullName;
                customer.PhoneNumber = customerViewModel.PhoneNumber;
                customer.Email = customerViewModel.Email;
                customer.DateOfBirth = customerViewModel.DateOfBirth;
                customer.Gender = customerViewModel.Gender;
                customer.IdentityCard = customerViewModel.IdentityCard;
                customer.IdentityCardIssueDate = customerViewModel.IdentityCardIssueDate;
                customer.IdentityCardIssuePlace = customerViewModel.IdentityCardIssuePlace;
                customer.Province = customerViewModel.Province;
                customer.District = customerViewModel.District;
                customer.Ward = customerViewModel.Ward;
                customer.Address = customerViewModel.Address;
                customer.BankAccount = customerViewModel.BankAccount;
                customer.BankName = customerViewModel.BankName;
                customer.Occupation = customerViewModel.Occupation;
                customer.Workplace = customerViewModel.Workplace;
                customer.EmergencyContact = customerViewModel.EmergencyContact;
                customer.EmergencyPhone = customerViewModel.EmergencyPhone;
                customer.Consultant = customerViewModel.Consultant;
                customer.ConsultantPhone = customerViewModel.ConsultantPhone;
                customer.AccessCode = customerViewModel.AccessCode;
                customer.IsForeigner = customerViewModel.IsForeigner;
                customer.Note = customerViewModel.Note;

                await _customerRepository.UpdateAsync(customer);
                TempData["SuccessMessage"] = "Cập nhật thông tin khách hàng thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Edit action: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                ModelState.AddModelError("", $"Lỗi khi cập nhật khách hàng: {ex.Message}");
                return View(customerViewModel);
            }
        }

        // GET: Customer/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _customerRepository.GetByIdAsync(id.Value);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer != null)
            {
                await _customerRepository.DeleteAsync(id);
            }
            return RedirectToAction(nameof(Index));
        }
    }
} 