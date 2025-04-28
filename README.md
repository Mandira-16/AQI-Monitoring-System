# AQI Monitoring System 🌍

## Project Overview 📊
The **Air Quality Index (AQI) Monitoring System** is a comprehensive web-based dashboard designed to monitor and visualize air quality in the Colombo Metropolitan Area 🌱. This system utilizes simulated air quality data collected from strategically placed sensors across Colombo, offering real-time monitoring and historical trend analysis 🔍. It serves environmental agencies, researchers, and the general public, providing critical insights that support data-driven decisions for a cleaner and healthier environment 🌿.

### Key Features 🌟
- **Real-Time Monitoring**: Displays live AQI data on an interactive map 🗺️ with color-coded markers representing different air quality levels (e.g., Good, Moderate, Unhealthy).
- **Historical Data Visualization**: Users can view AQI trends for the last 24 hours, week, or month 📅, helping to track air quality changes over time.
- **User Roles and Access Control**: Supports different user roles:
  - **Public Users**: No login required; access to the dashboard with real-time data and historical trends 👤.
  - **Monitoring Admins**: Secure login required; manage sensors, configure simulation parameters, and set alert thresholds 🔐.
  - **Data Providers**: Simulated AQI data is automatically generated and stored for historical analysis 📈.
- **Alert Configuration**: Set AQI level thresholds (e.g., Moderate, Unhealthy) to receive notifications when air quality reaches critical levels 🚨.

## Technologies Used 💻
- **Backend**: ASP.NET Core (C#) ⚙️
- **Database**: MySQL (Managed via MySQL Workbench) 🗄️
- **Frontend**: HTML, CSS, JavaScript 🎨
- **Map Integration**: Leaflet.js for interactive map visualization 🗺️
- **Testing**: Manual validation using Excel sheets 🧪

## Prerequisites ⚙️
Before setting up the project, ensure you have the following installed:
- **Visual Studio 2022** (Community, Professional, or Enterprise) with the **ASP.NET and web development** workload 💡.
- **MySQL Workbench 8.x** (Download [here](https://dev.mysql.com/downloads/workbench/)) 📦.
- **MySQL Server 8.x** (Included with MySQL Workbench or install separately) 🛠️.
- **Git** (Download [here](https://git-scm.com/)) for cloning the repository 🧑‍💻.
- A modern web browser (e.g., Chrome, Firefox) 🌐.

## Step-by-Step Setup Guide 🛠️

### 1. Clone the Repository 🖥️
Open your terminal (Command Prompt, PowerShell, or Git Bash) and clone the project repository:
```bash
git clone https://github.com/yourusername/AQI-Monitoring-System.git
cd AQI-Monitoring-System
```

### 2. Open the Project in Visual Studio 💻
1. Launch **Visual Studio 2022** 🖥️.
2. Go to **File** > **Open** > **Project/Solution** 📂.
3. Navigate to the AQI-Monitoring-System folder and select the `.sln` file (e.g., `AQI-Monitoring-System.sln`) 🔑.
4. Click **Open** to load the project 📥.

### 3. Set Up the MySQL Database 📊
1. Launch **MySQL Workbench** and connect to your MySQL Server (use the default user `root` or the user you configured) 🔐.
2. Import the database:
   - Go to **Server** > **Data Import** 🔄.
   - Select **Import from Self-Contained File** 🗂️.
   - Browse to the `Database/aqidatabase.sql` file in the project folder 📁.
   - Choose an existing schema or create a new one (e.g., `aqidatabase`) 🏗️.
   - Click **Start Import** to populate the database with data 📥.

3. Verify the Database:
   - In **MySQL Workbench**, expand the schema (e.g., `aqidatabase`) in the Navigator pane 🧐.
   - Ensure tables for sensors and AQI readings are present and contain data 🗃️.

### 4. Configure the Database Connection 🔧
1. Open the `appsettings.json` file in Visual Studio 📄.
2. Update the `ConnectionStrings` section with your MySQL server details:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost;Database=aqidatabase;User=root;Password=your_mysql_password;"
   }
   ```
3. Replace `your_mysql_password` with the password for your MySQL root user 🔑.
4. Save the `appsettings.json` file 💾.

### 5. Restore Dependencies 🔄
1. In Visual Studio, go to **Tools** > **NuGet Package Manager** > **Manage NuGet Packages for Solution** 📦.
2. Click **Restore** to download all required packages (e.g., Entity Framework Core, MySQL connectors) ⬇️.
   Alternatively, in the terminal, run:
   ```bash
   dotnet restore
   ```

### 6. Apply Database Migrations (If Applicable) 🔄
If your project uses **Entity Framework Core** migrations:
1. Open the **Package Manager Console** in Visual Studio (**Tools** > **NuGet Package Manager** > **Package Manager Console**) 🔧.
2. Run the following command to apply migrations:
   ```bash
   dotnet ef database update
   ```
   If no migrations are used, this step can be skipped (the `aqidatabase.sql` file already sets up the schema) 🚫.

### 7. Run the Application 🚀
1. In Visual Studio, set the ASP.NET Core project as the startup project 🎯.
   - Right-click the ASP.NET Core project in **Solution Explorer** (e.g., `AQI-Monitoring-System`) and select **Set as Startup Project** 🔨.
2. Press **F5** or click the **Start Debugging** button (green play arrow) ➡️ to run the project.
3. The application will launch in your default browser (e.g., at `https://localhost:5001` or `http://localhost:5000`) 🌐.

### 8. Usage 📊
- **Public Users**: No login required. Access the dashboard to view real-time AQI data and explore historical trends via the interactive map 🗺️.
- **Monitoring Admins**: Secure login required. Manage sensors, configure simulation parameters, and set AQI alert thresholds 🛠️.
- **Data Providers**: Simulated AQI data is automatically generated and stored for historical analysis 🔄.

### 9. Testing 🧪
The system was manually tested using Excel sheets to validate:
- The accuracy of AQI calculations for sensor locations 📊.
- Correct mapping of AQI data to color-coded markers on the Leaflet.js map 🗺️.
- The retrieval of historical data for 24-hour, weekly, and monthly periods 📅.

## Submission Notes 📦
The project is submitted as a `.zip` file containing:
- The **ASP.NET Core project source code** 💻.
- The `Database/aqidatabase.sql` file for MySQL 🗄️.
- This `README.md` with setup instructions 📘.

### Important:
- Install **MySQL Workbench** and **MySQL Server** to import `aqidatabase.sql` 🧑‍💻.
- Update the `appsettings.json` connection string with your MySQL credentials 🔑.
- Use **Visual Studio 2022** to restore dependencies and run the project 🚀.

## Contributing 🤝
To contribute, fork the repository on GitHub, make changes, and submit a pull request. Ensure that your contributions adhere to the project's coding standards and testing guidelines 📝.

## License 📜
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details ⚖️.

```
