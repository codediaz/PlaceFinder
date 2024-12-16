# 🛰 PlaceFinder Application 

## 🗺️ Project Description
PlaceFinder is a web application that integrates Google Maps and a third-party API (Foursquare) to allow users to search for places, save favorite locations, and provide suggestions for specific places. The application ensures an interactive and user-friendly experience with features such as displaying location details, saving places to favorites, and managing user comments (suggestions).


## 🚀 Features
1. **🔎 Search Places**
   - Search for places using a query and location.
   - Results displayed as markers on Google Maps.

2. **📍 View Place Details**
   - Detailed information about the location, including:
     - 🏷️ Name
     - 📫 Address
     - 📏 Distance
     - 🌐 Timezone
     - 💬 Suggestions
     - ❤️ Option to save to favorites.

3. **❤️ Save Places to Favorites**
   - Save places by clicking the "Save" button in the popup.
   - Places stored in the database for authenticated users.

4. **💬 Add and View Suggestions**
   - Add comments or suggestions for any place.
   - Suggestions are displayed under place details.
   - Automatic place saving before adding a suggestion.
   - Anonymous users can also contribute suggestions.

5. **⭐ View Saved Favorites**
   - Authenticated users can view their favorite places.
   - Display details like:
     - 🏷️ Name
     - 📏 Distance
     - 🌐 Timezone
     - 💬 Suggestions.

6. **🔐 User Authentication**
   - Save places to favorites only when logged in.
   - Suggestions available for anonymous users.

## 🛠️ Technology Stack
- **Frontend:** HTML, CSS (Bootstrap), JavaScript (ES6), Google Maps API 🌍
- **Backend:** ASP.NET Core MVC (C#) 🖥️
- **Database:** MySQL (via Railway) 🛢️
- **Third-party API:** Foursquare API for fetching place details 📡


## 🔌 API Endpoints
1. **Search Places** 🔎
   - Endpoint: `GET /Places/Search?query={query}&location={location}`

2. **Save Place** 💾
   - Endpoint: `POST /Places/SavePlace`
   - Payload:
     ```json
     {
       "placeId": "unique-id",
       "name": "Place Name",
       "distance": 1000,
       "timezone": "America/Guayaquil"
     }
     ```

3. **Add Suggestion** ✍️
   - Endpoint: `POST /Places/AddSuggestion`
   - Payload:
     ```json
     {
       "placeId": "unique-id",
       "content": "Suggestion content"
     }
     ```

4. **Get Suggestions** 📝
   - Endpoint: `GET /Places/GetSuggestions?placeId={placeId}`

5. **Get Favorites** 🌟
   - Endpoint: `GET /Places/GetFavorites`

## ⚙️ Installation and Setup
1. **Clone the Repository** 🛠️
   ```bash
   git clone https://github.com/your-repository-url.git
   cd PlaceFinder
   ```

2. **Setup the Database** 🛢️
   - Configure MySQL database and connection string in `appsettings.json`.

3. **Install Dependencies** 📦
   - Ensure .NET Core SDK is installed.
   - Run the following command:
     ```bash
     dotnet restore
     ```

4. **Run the Application** ▶️
   ```bash
   dotnet run
   ```
   - The application will run on `http://localhost:5172`.

5. **API Keys** 🔑
   - Add Google Maps API and Foursquare API keys in the application.

## 🛠️ Future Enhancements
- Add user authentication with JWT tokens.
- Implement pagination for suggestions.
- Enhance UI/UX for better user experience.
- Add a search history feature for logged-in users.

## 🗒️ Notes
- The project ensures that places are saved before suggestions are added to avoid "Unknown Place" entries.
- Anonymous users are assigned `UserId = 0` for suggestions.

## 🤝 **Credits**

<div align="center">
<a href="https://github.com/codediaz/PlaceFinder/graphs/contributors">
  <img align="center" src="https://contrib.rocks/image?repo=codediaz/PlaceFinder" alt="Contributors" style="margin: 10px auto; display: block; max-width: 80%; border-radius: 8px;" />
</a>
</div>

- **Developed By:** Sergio Díaz ✨
- **APIs Utilized:** 🌍 Google Maps (via GCP), 📍 Foursquare API
- **Database Hosted On:** 🛢️ Railway

## 📜 License
This project is licensed under the MIT License.
