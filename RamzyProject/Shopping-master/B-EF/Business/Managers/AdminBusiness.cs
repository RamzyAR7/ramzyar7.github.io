using A_Service.IUnitOfWork;
using A_Service.Models;
using A_Service.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B_EF.Business.Managers
{
    public interface IAdminBusiness
    {
        public Task<IdentityResult> CreateUser(UserDTO user);
        public Task<IdentityResult> ChangePassword(ChangePasswordDTO changePasswordDTO);
        public Task<List<UserDTO>> GetAllUsersAsync();
        Task<bool> EditUser(UserDTO userDTO);
        Task<UserDTO> GetUserById(string id);
        List<RoleDTO> GetAllRoles();
        RoleDTO GetRoleByID(string Id);

      //  Task<bool> CheckPhoneExist(string phone);
    }
    public class AdminBusiness : IAdminBusiness
    {
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        // private readonly ApplicationDBContext applicationDBContext;
        public AdminBusiness(
            
          UserManager<ApplicationUser> userManager,
          RoleManager<IdentityRole> roleManager, IMapper _mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            this._mapper = _mapper;

        }

        public RoleDTO GetRoleByID(string Id)
        {
            RoleDTO role = new RoleDTO();
            if (Id != null)
            {
                var result = _roleManager.FindByIdAsync(Id);
                if (result != null)
                {
                    role.Id = result.Result.Id;
                    role.Name = result.Result.Name;
                    return role;
                }
                else return role;
            }
            else return role;
        }
        public async Task<IdentityResult> CreateUser(UserDTO userDTO)
        {
            var aspnetuser = new ApplicationUser
            {
                UserName = userDTO.Username.ToString(),
                PhoneNumber = userDTO.PhoneNumber.ToString(),
            };

            try
            {
                var result = await _userManager.CreateAsync(aspnetuser, userDTO.Password);
                if (result.Succeeded)
                {
                    await this._userManager.AddToRoleAsync(aspnetuser, userDTO.RoleName);

                    //for (int i = 0; i < userDTO.Roles.Length; i++)
                    //{
                    //}

                }
                return result;
            }
            catch (Exception ex)
            {

                return null;
            }

        }

        public async Task<IdentityResult> ChangePassword(ChangePasswordDTO changePasswordDTO)
        {
            var user = await _userManager.FindByNameAsync(changePasswordDTO.UserName);
            IdentityResult result;
            if (user == null)
            {
                result = IdentityResult.Failed();
                return result;
            }
            else
            {
                result = await _userManager.ChangePasswordAsync(user, changePasswordDTO.OldPassword, changePasswordDTO.NewPassword);
                if (result.Succeeded)
                {
                    return result;
                }
                else return result;
            }
        }
        public async Task<List<UserDTO>> GetAllUsersAsync()
        {
            var allUsers = _userManager.Users.ToList();
            UserDTO userDTO = new UserDTO();
            RoleDTO[] roleDTOs;
            List<UserDTO> result = new List<UserDTO>();
            int counter = 0;
            foreach (var user in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                roleDTOs = new RoleDTO[roles.Count];
                userDTO = _mapper.Map<UserDTO>(user);
                counter = 0;
                foreach (var role in roles)
                {
                    roleDTOs[counter] = new RoleDTO { Name = role };
                    counter++;
                }
                userDTO.Roles = roleDTOs;
                result.Add(userDTO);

            }

            return result;
        }
        public async Task<bool> EditUser(UserDTO userDTO)
        {
            var users = await _userManager.FindByNameAsync(userDTO.Username);
            var userRoles = await _userManager.GetRolesAsync(users);
            var allRoles = _roleManager.Roles.ToList();
            users.UserName = userDTO.Username;
            users.PhoneNumber = userDTO.PhoneNumber;

            var selectedRoles = userDTO.Roles.Where(x => x.Selected).Select(x => x.Name);
            var removeResult = await _userManager.RemoveFromRolesAsync(users, userRoles);

            if (!removeResult.Succeeded)
            {
            }
            var addResult = await _userManager.AddToRolesAsync(users, selectedRoles);
            if (!addResult.Succeeded)
            {
            }


            var result = await _userManager.UpdateAsync(users);

            if (result.Succeeded)
            {
                return true;
            }

            return false;
        }
        public async Task<UserDTO> GetUserById(string id)
        {
            var viewModel = new UserDTO();

            var users = await _userManager.FindByIdAsync(id);
            var allRoles = _roleManager.Roles.ToList();

            var userRoles = await _userManager.GetRolesAsync(users);

            viewModel = new UserDTO()
            {
                Id = users.Id,
                Username = users.UserName,
                PhoneNumber = users.PhoneNumber,
          
                Roles = allRoles.Select(
                   x => new RoleDTO()
                   {
                       Id = x.Id,
                       Name = x.Name,
                       Selected = userRoles.Contains(x.Name)
                   }).ToArray(),





            };
            return viewModel;
        }

        public List<RoleDTO> GetAllRoles()
        {
            var roleViewModel = new List<RoleDTO>();
            try
            {
                var roles = _roleManager.Roles.ToList();
                foreach (var item in roles)
                {
                    roleViewModel.Add(new RoleDTO()
                    {
                        Id = item.Id,
                        Name = item.Name,
                    });
                }
            }
            catch (Exception ex)
            {

            }

            return roleViewModel;
        }

       
    }
}
