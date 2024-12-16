let map;
let markers = [];

// Inicializar el mapa
function initMap() {
    map = new google.maps.Map(document.getElementById("map"), {
        center: { lat: -2.170998, lng: -79.922359 }, // Guayaquil
        zoom: 12,
    });
}

// Agregar marcador con ícono personalizado e infowindow
function addMarker(place) {
    const position = {
        lat: place.geocodes.main.latitude,
        lng: place.geocodes.main.longitude
    };

    // Construir ícono personalizado (si existe)
    const iconUrl = place.categories?.[0]?.icon?.prefix + "bg_32" + place.categories?.[0]?.icon?.suffix || null;

    const marker = new google.maps.Marker({
        position: position,
        map: map,
        title: place.name,
        icon: iconUrl || "http://maps.google.com/mapfiles/ms/icons/red-dot.png" // Ícono por defecto
    });

    // Infowindow con detalles y botón de guardar
    const infoWindowContent = `
        <div>
            <h6>${place.name}</h6>
            <p>Distance: ${place.distance} meters</p>
            <button onclick="savePlace('${place.fsq_id}')" class="btn btn-primary btn-sm">
                Save to Favorites
            </button>
        </div>
    `;

    const infoWindow = new google.maps.InfoWindow({
        content: infoWindowContent
    });

    marker.addListener("click", () => {
        infoWindow.open(map, marker);
    });

    markers.push(marker);
}


// Limpiar marcadores existentes
function clearMarkers() {
    markers.forEach(marker => marker.setMap(null));
    markers = [];
}

// Buscar lugares y agregar marcadores
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
            alert(result.message || "No places found.");
        }
    } catch (error) {
        console.error("Error fetching places:", error);
        alert("An unexpected error occurred.");
    }
}

// Mostrar lugares en el mapa
function plotPlacesOnMap(places) {
    clearMarkers();
    if (places.length === 0) {
        alert("No places found.");
        return;
    }

    // Centrar mapa en el primer lugar
    setCenter(places);

    // Agregar marcadores
    places.forEach(place => {
        if (place.geocodes && place.geocodes.main) {
            addMarker(place);
        }
    });
}

// Centrar el mapa
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

async function savePlace(placeId) {
    try {
        const response = await fetch(`/Places/SavePlace`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({ placeId: placeId }) // Enviar como un objeto JSON
        });

        const result = await response.json();

        if (response.ok && result.success) {
            alert(result.message || "Place saved successfully!");
        } else {
            alert(result.message || "Failed to save place.");
        }
    } catch (error) {
        console.error("Error saving place:", error);
        alert("An unexpected error occurred.");
    }
}


