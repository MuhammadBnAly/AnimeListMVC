using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AnimeList_MVC_Identity.Models.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AnimeList_MVC_Identity.Consts;

namespace AnimeList_MVC_Identity.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        //private myConsts _myConsts = new myConsts();

        public IndexModel(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            // _myConsts = myConsts;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {

            [Required, Display(Name = "First Name")]
            public string FirstName { get; set; }

            
            [Required, Display(Name = "Last Name")]
            public string LastName { get; set; }


            [Required, Display(Name = "Username")]
            public string Username { get; set; }


            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            [Display(Name ="Profile Picture")]
            public byte[] ProfilePicture  { get; set; }

        }

        private async Task LoadAsync(AppUser user)
        {
            //var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            //Username = userName;

            Input = new InputModel
            {
                Username = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = phoneNumber,
                ProfilePicture = user.ProfilePicture
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            //var username = await _userManager.GetUserNameAsync(user);
            var username = user.UserName;
            var firstname = user.FirstName;
            var lastname = user.LastName;

            if (Input.Username != username)
            {
                user.UserName = Input.Username;
                await _userManager.UpdateAsync(user);
            }
            if (Input.FirstName != firstname)
            {
                user.FirstName = Input.FirstName;
                await _userManager.UpdateAsync(user);
            }
            if (Input.LastName != lastname)
            {
                user.LastName = Input.LastName;
                await _userManager.UpdateAsync(user);
            }

            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            // Profile Picture
            if (Request.Form.Files.Count > 0)
            {
                var file = Request.Form.Files.FirstOrDefault();


                // convert picture to arr
                var dataStream = new MemoryStream();
                using (dataStream)
                {
                    await file.CopyToAsync(dataStream);
                    user.ProfilePicture = dataStream.ToArray();
                }
                await _userManager.UpdateAsync(user);


                // check if image is jpg / png
                var allowedExtensions = new List<string> { ".jpg", ".png" };
                if (!allowedExtensions.Contains(Path.GetExtension(file.FileName).ToLower()))
                {
                    //ModelState.AddModelError("ProfilePicture", "Only .JPG , .PNG images are allowed.");
                    StatusMessage = "Only .JPG , .PNG images are allowed.";
                    return RedirectToPage();
                }

                // check the size of the image : OneMegaByte = 1 MB = 1048576 B
                var OneMegaByte = 1048576;
                if (file.Length > OneMegaByte)
                {
                    //var errorMsg = "<div class='alert alert-danger' role='alert'> Profile Picture Can't be more than 1 MB. </ div > ";

                    //ModelState.AddModelError("ProfilePicture", "Profile Picture Can't be more than 1 MB.");
                    StatusMessage = "Profile Picture Can't be more than 1 MB.";
                    return RedirectToPage();
                }

            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
