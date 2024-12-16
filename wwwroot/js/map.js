let map;
let markers = [];

function initMap() {
    map = new google.maps.Map(document.getElementById("map"), {
        center: { lat: -2.170998, lng: -79.922359 }, // Guayaquil
        zoom: 12,
    });
}

function addMarker(position, title) {
    const marker = new google.maps.Marker({
        position: position,
        map: map,
        title: title,
    });
    markers.push(marker);
}

function clearMarkers() {
    markers.forEach(marker => marker.setMap(null)); 
    markers = []; 
}

async function searchPlaces() {
    const query = document.getElementById("query").value;
    const location = document.getElementById("location").value;

    if (!query || !location) {
        alert("Please provide both query and location.");
        return;
    }

    try {
        const response = await fetch(`/Places/Search?query=${query}&location=${location}`);
        const result = await response.json();

        if (result.success && Array.isArray(result.data)) {
            plotPlacesOnMap(result.data);
        } else {
            console.warn("No places found or error occurred:", result.message);
            alert(result.message || "No places found.");
        }
    } catch (error) {
        console.error("Error fetching places:", error);
        alert("An unexpected error occurred.");
    }
}

async function savePlace(placeId) {
    try {
        const response = await fetch(`/Places/SavePlace`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ placeId })
        });

        if (response.status === 401) {
            // Redirigir al login si no está autenticado
            window.location.href = "/Account/Login";
            return;
        }

        const result = await response.json();
        alert(result.message || "Place saved!");
    } catch (error) {
        console.error("Error saving place:", error);
        alert("Failed to save place.");
    }
}

function plotPlacesOnMap(places) {

    clearMarkers();

    if (places.length === 0) {
        alert("No places found.");
        return;
    }

    setCenter(places);

    places.forEach(place => {
        if (place.geocodes && place.geocodes.main) {
            const position = {
                lat: place.geocodes.main.latitude,
                lng: place.geocodes.main.longitude
            };
            addMarker(position, place.name);
        } else {
            console.warn("Missing coordinates for place:", place);
        }
    });
}

function setCenter(places) {
    const firstPlace = places[0];
    if (firstPlace.geocodes && firstPlace.geocodes.main) {
        const centerPosition = {
            lat: firstPlace.geocodes.main.latitude,
            lng: firstPlace.geocodes.main.longitude
        };

        map.setCenter(centerPosition);
        map.setZoom(12);
    }
}