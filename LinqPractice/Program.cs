using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

class Program
{
    class Employee
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string Department { get; set; }
        public double Salary { get; set; }
        public bool IsPermanent { get; set; }
    }

    public class Department
    {
        public string Name { get; set; }
        public string Location { get; set; }
    }

    static void Main()
    {
        List<int> numbers = new List<int> { 10, 15, 20, 25, 30, 35, 40, 45, 50 };
        var evenNumbers = from number in numbers
                          where number % 2 == 0
                          select number;
        foreach (var number in evenNumbers)
        {
            Console.WriteLine(number);
        }

        List<string> names = new List<string> { "Ali", "Ahmed", "Sara", "Ayesha", "John" };

        var result = from name in names
                     where name.StartsWith("A")
                     select name;
        Console.WriteLine("\nName starting with A");

        foreach (var name in result)
        {
            Console.WriteLine(name);
        }
        var sortedNames = from name in names
                          orderby name descending
                          select name;
        Console.WriteLine("\nSorted Names alphabatically");
        foreach (var name in sortedNames)
        {
            Console.WriteLine(name);
        }

        Console.WriteLine("\nEmployee Names:");

        List<Employee> employees = new List<Employee>
        {
            new Employee { Name = "Ali", Age = 28, Department = "IT", Salary = 55000, IsPermanent = true },
            new Employee { Name = "Sara", Age = 24, Department = "IT", Salary = 40000, IsPermanent = false },
            new Employee { Name = "Elanore", Age = 26, Department = "Accounts", Salary = 45000, IsPermanent = false },
            new Employee { Name = "Michael", Age = 31, Department = "HR", Salary = 50000, IsPermanent = true },
            new Employee { Name = "Martin", Age = 27, Department = "Accounts", Salary = 40000, IsPermanent = false },
            new Employee { Name = "John", Age = 30, Department = "Finance", Salary = 60000, IsPermanent = true }
        };

        var departments = new List<Department>
{
    new Department { Name = "IT", Location = "Building A" },
    new Department { Name = "Accounts", Location = "Building B" },
    new Department { Name = "HR", Location = "Building C" },
    new Department { Name = "Finance", Location = "Building D" }
};

        var empNames = from emp in employees
                       select emp.Name;
        foreach (var emp in empNames)
        {
            Console.WriteLine(emp);
        }
        Console.WriteLine("\nEmployees having age more than 27");
        var empAge = from emp in employees
                     where emp.Age > 27
                     select new { emp.Name, emp.Age }; // For selecting only specific fields in an object - lightweight
        foreach (var emp in empAge)
        {
            Console.WriteLine(emp.Name + " : " + emp.Age);
            //Console.WriteLine($"{emp.Name} - {emp.Age}");
        }
        Console.WriteLine("\nEmployees sorted by Salaray");
        var empSlaary = from emp in employees
                        orderby emp.Salary
                        select new { emp.Name, emp.Salary };
        foreach (var emp in empSlaary)
        {
            Console.WriteLine($"{emp.Name} - {emp.Salary}");
        }

        //Using Linq Method Syntax
        Console.WriteLine("\nEmployees sorted by Salary and having IT Department, using LINQ MEthod Syntax");

        var empDep = employees
                     .Where(emp => emp.Department == "IT")
                     .OrderByDescending(emp => emp.Salary)
                     .Select(emp => new { emp.Name, emp.Department, emp.Salary });
        foreach (var emp in empDep)
        {
            Console.WriteLine($"{emp.Name} - {emp.Department} - {emp.Salary}");
        }

        Console.WriteLine("\nEmployees grouped by Department:");

        var groupedEmpDep = from emp in employees
                            group emp by emp.Department into deptGroup
                            select deptGroup;
        foreach (var group in groupedEmpDep)
        {
            Console.WriteLine($"\nDepartment - {group.Key}");
            foreach (var emp in group)
            {
                Console.WriteLine($"{emp.Name} - {emp.Salary}");
            }
        }

        // Grouping using Method Syntax
        Console.WriteLine("\nEmployees grouped by Method Syntax:");

        var depGroup = employees
            .GroupBy(emp => emp.Department);
        foreach (var group in depGroup)
        {
            Console.WriteLine($"Department: {group.Key}");
            foreach (var emp in group)
            {
                Console.WriteLine($"{emp.Name} - {emp.Salary}");
            }
        }

        Console.WriteLine("\nEmployees grouped by their Permanent Status");

        var status = employees
            .Where(emp => emp.IsPermanent)
            .GroupBy(emp => emp.Department)
            .Select(group => new
            {
                Department = group.Key,
                Count = group.Count()
            });
        foreach (var emp in status)
        {
            Console.WriteLine($"{emp.Department} - {emp.Count}");
        }
        Console.WriteLine("\nEmployees grouped by their Temporary Status");
        var queryStatus = from emp in employees
                          where emp.IsPermanent == false
                          group emp by emp.Department into tempEmployees
                          select new
                          {
                              Department = tempEmployees.Key,
                              Count = tempEmployees.Count(),
                              Name = tempEmployees.Select(emp => emp.Name).ToList()
                          };

        foreach (var emp in queryStatus)
        {
            Console.WriteLine($"{emp.Department} - {emp.Count} - {string.Join(",", emp.Name)}");
        }

        var depCombQuery = employees.Join(
            departments,
            emp => emp.Department,
            dept => dept.Name,
            (emp, dept) => new
            {
                EmployeeName = emp.Name,
                Department = dept.Name,
                Location = dept.Location,
                Salary = emp.Salary
            });

        foreach (var emp in depCombQuery)
        {
            Console.WriteLine($"Employee {emp.EmployeeName} works in {emp.Department}, in building {emp.Location}, and has salary {emp.Salary}");
        }

        Console.WriteLine("\nGroupJoin: Departments and their Employees:");

        var deptWithEmp = departments.GroupJoin(
            employees,
            dept => dept.Name,
            emp => emp.Department,
            (dept, emp) => new
            {
                Department = dept.Name,
                Location = dept.Location,
                Employees = emp
            });
        foreach (var group in deptWithEmp)
        {
            Console.WriteLine($"Department: {group.Department} - {group.Location}");
            foreach (var emp in group.Employees)
            {
                Console.WriteLine($"- {emp.Name}, Age: {emp.Age}, Salary: {emp.Salary}");
            }
        }

        Console.WriteLine("\n Using IQueryable for future database style queries");

        // Creating IQueryable collections from existing lists
        IQueryable<Employee> queryableEmployees = employees.AsQueryable();
        IQueryable<Department> queryableDepartments = departments.AsQueryable();

        // Find permanent employees with salary > 45000 and show department location
        var highSalaryQuery = queryableEmployees
            .Where(emp => emp.IsPermanent && emp.Salary > 45000)
            .Join(queryableDepartments,
                  emp => emp.Department,
                  dept => dept.Name,
                  (emp, dept) => new
                  {
                      emp.Name,
                      emp.Salary,
                      emp.Department,
                      dept.Location
                  });

        foreach (var item in highSalaryQuery)
        {
            Console.WriteLine($"Name: {item.Name}, Department: {item.Department}, Salary: {item.Salary}, Location: {item.Location}");
        }

        Console.WriteLine("\n Using IQueryable with GroupJoin (like LEFT JOIN)");

        var iqGroupJoin = queryableDepartments
    .GroupJoin(
        queryableEmployees,
        dept => dept.Name,
        emp => emp.Department,
        (dept, empGroup) => new
        {
            Department = dept.Name,
            Location = dept.Location,
            Employees = empGroup
        }
    );

        foreach (var group in iqGroupJoin)
        {
            Console.WriteLine($"\nDepartment: {group.Department} - Location: {group.Location}");

            if (group.Employees.Any())
            {
                foreach (var emp in group.Employees)
                {
                    Console.WriteLine($"{emp.Name}, Age: {emp.Age}, Salary: {emp.Salary}, Permanent: {emp.IsPermanent}");
                }
            }
            else
            {
                Console.WriteLine("No employees in this department");
            }
        }

        Console.WriteLine("\n Using IQueryable with SelectMany");

        var flatJoin = departments
    .SelectMany(
        dept => employees.Where(emp => emp.Department == dept.Name),
        (dept, emp) => new
        {
            EmployeeName = emp.Name,
            Department = dept.Name
        });

        Console.WriteLine("\nUsing SelectMany (Flat Join):");
        foreach (var item in flatJoin)
        {
            Console.WriteLine($"{item.EmployeeName} works in {item.Department}");
        }


        // Deffered and Immediate running concept

        var query = employees.Where(emp => emp.Salary > 45000);

        Console.WriteLine("Query created but not run yet");

        foreach (var emp in query)  // Execution happens here
        {
            Console.WriteLine($"{emp.Name} - {emp.Salary}");
        }

        // Immediate execution example:
        var list = employees.Where(emp => emp.Salary > 45000).ToList();  // Runs now
        Console.WriteLine("\nImmediate execution done with ToList(), count: " + list.Count);


        // Any() method
        bool hasITEmployee = employees.Any(emp => emp.Department == "IT");
        Console.WriteLine(hasITEmployee);  // true or false

        // All() method

        bool allHighSalary = employees.All(emp => emp.Salary > 30000);
        Console.WriteLine(allHighSalary);  // true or false

        // first() method

        var firstITEmployee = employees.First(emp => emp.Department == "IT");
        Console.WriteLine(firstITEmployee.Name);

        // FirstOrDefault() method

        var firstMarketingEmp = employees.FirstOrDefault(emp => emp.Department == "Marketing");
        if (firstMarketingEmp != null)
            Console.WriteLine(firstMarketingEmp.Name);
        else
            Console.WriteLine("No employee in Marketing");

        // Single() method

        var john = employees.Single(emp => emp.Name == "John");
        Console.WriteLine(john.Department);

        // ToList() method

        var highSalaryEmployees = employees
    .Where(emp => emp.Salary > 50000)
    .ToList(); // Now it's a List<Employee>

        Console.WriteLine(highSalaryEmployees.Count);

        // ToArray() method

        var empArray = employees
    .Where(emp => emp.Age < 30)
    .ToArray(); // Now it's an array of Employee

        Console.WriteLine(empArray.Length);






    }
}
