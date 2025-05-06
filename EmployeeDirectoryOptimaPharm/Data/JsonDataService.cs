using EmployeeDirectoryOptimaPharm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Win32;
using System.Windows;

namespace EmployeeDirectoryOptimaPharm.Data
{
    public class ImportResult<T>
    {
        public bool Success { get; set; }
        public List<T> Data { get; set; } = new List<T>();
        public string? FilePath { get; set; }
        public string? ErrorMessage { get; set; }
    }
    public class ExportResult
    {
        public bool Success { get; set; }
        public string? FilePath { get; set; }
    }
    public interface IJsonDataService
    {
        Task<ImportResult<Employee>> ImportEmployeesFromJsonAsync();
        Task<ExportResult> ExportEmployeesToJsonAsync(List<Employee> employees);
    }

    public class JsonDataService : IJsonDataService
    {
        public async Task<ImportResult<Employee>> ImportEmployeesFromJsonAsync()
        {
            var dialog = new OpenFileDialog
            {
                Title = "Open JSON File",
                Filter = "JSON files (*.json)|*.json",
                Multiselect = false
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    string json = await File.ReadAllTextAsync(dialog.FileName);
                    var employees = JsonSerializer.Deserialize<List<Employee>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        ReferenceHandler = ReferenceHandler.IgnoreCycles
                    });

                    return new ImportResult<Employee>
                    {
                        Success = true,
                        Data = employees ?? new List<Employee>(),
                        FilePath = dialog.FileName
                    };
                }
                catch (Exception ex)
                {
                    return new ImportResult<Employee>
                    {
                        Success = false,
                        ErrorMessage = ex.Message,
                        FilePath = dialog.FileName
                    };
                }
            }

            return new ImportResult<Employee>
            {
                Success = false,
                ErrorMessage = "No file was selected."
            };
        }
        public async Task<ExportResult> ExportEmployeesToJsonAsync(List<Employee> employees)
        {
            var dialog = new SaveFileDialog
            {
                Title = "Save JSON File",
                Filter = "JSON files (*.json)|*.json",
                FileName = $"Employees_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.json",
                DefaultExt = ".json",
                AddExtension = true
            };

            if (dialog.ShowDialog() == true)
            {
                string json = JsonSerializer.Serialize(employees, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });

                await File.WriteAllTextAsync(dialog.FileName, json);


                return new ExportResult
                {
                    Success = true,
                    FilePath = dialog.FileName
                };
            }
            else
            {
                return new ExportResult
                {
                    Success = false,
                    FilePath = null
                };
            }
        }
    }
}
