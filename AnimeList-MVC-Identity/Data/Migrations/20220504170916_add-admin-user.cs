using Microsoft.EntityFrameworkCore.Migrations;

namespace AnimeList_MVC_Identity.Data.Migrations
{
    public partial class addadminuser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO [Security].[users] ([Id], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [FirstName], [LastName], [ProfilePicture]) VALUES (N'd0996a40-eb23-468a-9f27-74e5eba1c7a7', N'admin', N'Admin', N'admin@test.com', N'ADMIN@TEST.COM', 0, N'AQAAAAEAACcQAAAAEDnY19b7lZZr4n5KBTEKMhMzeRFmjh+mmtBwUEJHfh6UUXnkjH+IrhGDAYXZbCmMPQ==', N'EL6WHNYTXZ6SGYHBH3YDUJFONVJSQZST', N'bc2bdd97-bc1a-47fa-8ab3-239ee07da903', NULL, 0, 0, NULL, 1, 0, N'Admin', N'Admin', null )");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM [Security].[users] Where Id='d0996a40-eb23-468a-9f27-74e5eba1c7a7'");
        }
    }
}
