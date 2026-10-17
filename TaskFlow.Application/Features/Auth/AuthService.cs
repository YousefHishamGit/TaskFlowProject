using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Exceptions;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Exceptions;

namespace TaskFlow.Application.Features.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IApplicationDbContext _context;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly ICurrentUser _currentUser;

        public AuthService(
            IApplicationDbContext context,
            IPasswordHasher passwordHasher,
            IJwtTokenService jwtTokenService,
            ICurrentUser currentUser)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _jwtTokenService = jwtTokenService;
            _currentUser = currentUser;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
        {
            
            var normalizedEmail = request.Email?.Trim().ToLowerInvariant();

         
            var isEmailTaken = await _context.Users
                .AnyAsync(u => u.Email == normalizedEmail);

            if (isEmailTaken)
            {
                throw new ConflictException("Email is already registered");
            }

       
            if (string.IsNullOrWhiteSpace(request.Password))
            {
                throw new DomainException("Password is required.");
            }

            var passwordHash = _passwordHasher.Hash(request.Password);

           
            var user = new User(request.FullName, request.Email, passwordHash);

            
            _context.Users.Add(user);
            await _context.SaveChangesAsync(cancellationToken);

         
            var token = _jwtTokenService.GenerateToken(user);
            var userDto = ToDto(user);

            return new AuthResponse(token, userDto);
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                throw new UnauthorizedException("Invalid email or password");
            }
            var normalizedEmail = request.Email.Trim().ToLowerInvariant();
            var user = await _context.Users
                                .AsNoTracking()
                                .FirstOrDefaultAsync(u => u.Email == normalizedEmail);
            if (user is null)
            {
                throw new UnauthorizedException("Invalid email or password");
            }
            var isPasswordValid = _passwordHasher.Verify(request.Password, user.PasswordHash);

            if (!isPasswordValid)
            {
                throw new UnauthorizedException("Invalid email or password");
            }
            var token = _jwtTokenService.GenerateToken(user);
            var userDto = ToDto(user);

            return new AuthResponse(token, userDto);
        }

        public async Task<UserDto> GetCurrentUserAsync(CancellationToken cancellationToken = default)
        {
            var user = await _context.Users
                        .AsNoTracking()
                        .FirstOrDefaultAsync(u => u.Id == _currentUser.UserId, cancellationToken);

            
            if (user is null)
            {
                throw new NotFoundException("User not found");
            }

         
            return ToDto(user);
        }

        
        private static UserDto ToDto(User user)
        {
            return new UserDto(
                user.Id,
                user.FullName,
                user.Email,
                user.Role.ToString()
            );
        }
    }
}