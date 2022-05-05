using Microsoft.EntityFrameworkCore.Migrations;

namespace AnimeList_MVC_Identity.Data.Migrations
{
    public partial class assignadminusertoallroles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("insert into [Security].[UserRoles] (UserId, RoleId) select 'd0996a40-eb23-468a-9f27-74e5eba1c7a7', Id From[Security].[Roles]");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("Delete From [Security].[UserRoles] Where UserId = 'd0996a40-eb23-468a-9f27-74e5eba1c7a7'");
        }
    }
}
