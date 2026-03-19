using ETD_Portal.Data;
using ETD_Portal.HR_Management.BLL.Classes;
using ETD_Portal.HR_Management.BLL.Interfaces;
using ETD_Portal.HR_Management.DAL.Classes;
using ETD_Portal.HR_Management.DAL.Interfaces;
using ETD_Portal.Reimbursement_Mgmt.BLL.Classes;
using ETD_Portal.Reimbursement_Mgmt.BLL.Interfaces;
using ETD_Portal.Reimbursement_Mgmt.DAL.Classes;
using ETD_Portal.Reimbursement_Mgmt.DAL.Interfaces;
using ETD_Portal.Reservation_Mgmt.BLL.Classes;
using ETD_Portal.Reservation_Mgmt.BLL.Interfaces;
using ETD_Portal.Reservation_Mgmt.DAL.Classes;
using ETD_Portal.Reservation_Mgmt.DAL.Interfaces;
using ETD_Portal.TravelPlanner.BLL.Classes;
using ETD_Portal.TravelPlanner.BLL.Interfaces;
using ETD_Portal.TravelPlanner.DAL.Classes;
using ETD_Portal.TravelPlanner.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<ETDPortalDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ETDPortalDb")));

builder.Services.AddScoped<IGradeRepo, GradeRepo>();
builder.Services.AddScoped<IUserRepo, UserRepo>();
builder.Services.AddScoped<IGradeHistoryRepo, GradeHistoryRepo>();

builder.Services.AddScoped<IGradeServices, GradeServices>();
builder.Services.AddScoped<IUserServices, UserServices>();

builder.Services.AddScoped<ILocationRepo, LocationRepo>();
builder.Services.AddScoped<ITravelRequestRepo, TravelRequestRepo>();
builder.Services.AddScoped<ITravelBudgetRepo, TravelBudgetRepo>();

builder.Services.AddScoped<ILocationServices, LocationServices>();
builder.Services.AddScoped<ITravelRequestServices, TravelRequestServices>();
builder.Services.AddScoped<IBudgetAllocationServices, BudgetAllocationServices>();

builder.Services.AddScoped<IReservationTypeRepo, ReservationTypeRepo>();
builder.Services.AddScoped<IReservationRepo, ReservationRepo>();
builder.Services.AddScoped<IReservationDocRepo, ReservationDocRepo>();

builder.Services.AddScoped<IReservationTypeServices, ReservationTypeServices>();
builder.Services.AddScoped<IReservationServices, ReservationServices>();
builder.Services.AddScoped<IReservationDocServices, ReservationDocServices>();

builder.Services.AddScoped<IReimbursementTypeRepo, ReimbursementTypeRepo>();
builder.Services.AddScoped<IReimbursementTypeServices, ReimbursementTypeServices>();

builder.Services.AddScoped<IReimbursementRepo, ReimbursementRepo>();
builder.Services.AddScoped<IReimbursementServices, ReimbursementServices>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000") // React app URL
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowReactApp");
app.UseAuthorization();

app.MapControllers();

app.Run();
