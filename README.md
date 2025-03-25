# AQI Monitoring System

## Project Overview
The **Air Quality Index (AQI) Monitoring System** is a real-time web-based dashboard designed to monitor air quality in the Colombo Metropolitan Area. This system visualizes simulated air quality data from strategically placed sensors across Colombo, allowing users to monitor air quality in real-time and explore historical trends. 

The goal of this project is to create an interactive platform for environmental agencies, researchers, and the public to stay informed about air quality and support data-driven actions for a cleaner environment.

### Key Features:
- **Real-Time Monitoring**: View real-time AQI data on an interactive map with color-coded markers to represent air quality levels.
- **Historical Data Visualization**: Explore AQI trends over time, with the ability to display data for the last 24 hours, week, or month.
- **User Roles**: The system supports different user roles, including Monitoring Admins, Data Providers, and Public Users.
- **Alert Configuration**: Set alert thresholds for various AQI levels (e.g., Moderate, Unhealthy) to notify users of significant air quality changes.

## Project Setup

### Prerequisites
Before setting up this project, ensure that you have the following installed on your local machine:

- [PHP (Laravel recommended) or ASP.NET Core (C#)](https://www.php.net/) for the backend
- [MySQL/PostgreSQL/SQL Server](https://www.mysql.com/) for the database
- [Leaflet/Google Maps API](https://leafletjs.com/) for map integration
- Frontend technologies: **HTML**, **CSS**, and **JavaScript**

### Installation

1. **Clone the repository:**
   ```bash
   git clone https://github.com/yourusername/AQI-Monitoring-System.git
   cd AQI-Monitoring-System
   ```

2. **Install dependencies:**
   - For **Laravel**: 
     ```bash
     composer install
     ```
   - For **ASP.NET Core**:
     ```bash
     dotnet restore
     ```

3. **Set up the database:**
   - Create the necessary database in MySQL/PostgreSQL.
   - Configure the database settings in the `.env` file or `appsettings.json`.
   - Run the migrations to create the necessary tables:
     - For **Laravel**:
       ```bash
       php artisan migrate
       ```
     - For **ASP.NET Core**:
       ```bash
       dotnet ef database update
       ```

4. **Run the application:**
   - For **Laravel**:
     ```bash
     php artisan serve
     ```
   - For **ASP.NET Core**:
     ```bash
     dotnet run
     ```

5. The system will now be accessible on your local development server.

### Usage
1. **For Public Users**: No login is required. Users can view real-time AQI data and historical trends by selecting a sensor location on the map.
2. **For Monitoring Admins**: Secure login required for managing simulated sensors, configuring data simulation parameters, and setting alert thresholds.
3. **For Data Providers**: The system will automatically generate simulated AQI data at regular intervals, which will be stored for historical analysis.

## Technologies Used
- **Backend**: PHP (Laravel) / C# (ASP.NET Core)
- **Database**: MySQL / PostgreSQL / SQL Server
- **Frontend**: HTML, CSS, JavaScript
- **Map Integration**: Leaflet / Google Maps API

## Contributing
This project is a team effort. To contribute, fork the repository and submit a pull request with your changes. Ensure all changes follow the coding standards and testing guidelines specified in the project documentation.

## License
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
```

This markdown version contains the same content as before but formatted in Markdown syntax, which you can directly use in your GitHub repository README file.
