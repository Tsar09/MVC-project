using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Claims;
using System.Threading.Tasks;
using VideoSharing.Business.DTO;
using VideoSharing.Business.Infrastructure;

namespace VideoSharing.Business
{
    public interface IUserService : IDisposable
    {
        Task<OperationDetails> Create(AccountDTO userDto);
        Task<ClaimsIdentity> Authenticate(AccountDTO userDto);
        Task SetInitialData(AccountDTO adminDto, List<string> roles);
        Task ConfirmEmail(string Token, string Email);
        Task<OperationDetails> Delete(string id);
        Task<OperationDetails> Edit(ClientProfileDTO id);
        Task<UserInfoDTO> ForgotPassword(ForgotPasswordDTO dto);
        Task<OperationDetails> ResetPassword(PasswordResertDTO model);
    }
}
