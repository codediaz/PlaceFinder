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

    const iconUrl = place.categories?.[0]?.icon?.prefix + "bg_32" + place.categories?.[0]?.icon?.suffix || null;

    const marker = new google.maps.Marker({
        position: position,
        map: map,
        title: place.name,
        icon: iconUrl || "http://maps.google.com/mapfiles/ms/icons/red-dot.png"
    });

    // HTML del InfoWindow
    const infoWindowContent = `
        <div>
            <h6>${place.name}</h6>
            <p>Distance: ${place.distance} meters</p>
            <p>Timezone: ${place.timezone || "N/A"}</p>
            <button onclick="savePlace('${place.fsq_id}', '${place.name}', ${place.distance}, '${place.timezone}')"
                    class="btn btn-primary btn-sm mb-2">
                Save to Favorites
            </button>
            <hr/>
            <!-- Sugerencias -->
            <h6>Suggestions:</h6>
            <div id="suggestions-container-${place.fsq_id}" class="mb-2 text-muted">
                Loading suggestions...
            </div>
            <!-- Formulario de sugerencias -->
            <textarea id="suggestion-input-${place.fsq_id}" class="form-control mb-2" rows="2"
                      placeholder="Write your suggestion here..."></textarea>
            <button class="btn btn-success btn-sm"
                    onclick="addSuggestion('${place.fsq_id}')">
                Submit Suggestion
            </button>
        </div>
    `;

    const infoWindow = new google.maps.InfoWindow({
        content: infoWindowContent
    });

    marker.addListener("click", () => {
        infoWindow.open(map, marker);
        loadSuggestions(place.fsq_id); // Cargar sugerencias existentes
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

async function savePlace(placeId, name, distance, timezone) {
    try {
        const response = await fetch(`/Places/SavePlace`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                placeId: placeId,
                name: name,
                distance: distance,
                timezone: timezone
            })
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



async function loadSuggestions(placeId) {
    try {
        const response = await fetch(`/Places/GetSuggestions?placeId=${placeId}`);
        const result = await response.json();

        const container = document.getElementById(`suggestions-container-${placeId}`);
        if (result.success && result.data.length > 0) {
            const suggestionsHtml = result.data.map(s => `
                <div class="alert alert-light p-1">
                    <p>${s.content}</p>
                    <small>${s.userId === 0 ? "Anonymous" : `By User #${s.userId}`} on ${new Date(s.createdAt).toLocaleString()}</small>
                </div>
            `).join("");
            container.innerHTML = suggestionsHtml;
        } else {
            container.innerHTML = `<p class='text-muted'>No suggestions available for this place.</p>`;
        }
    } catch (error) {
        console.error("Error loading suggestions:", error);
        document.getElementById(`suggestions-container-${placeId}`).innerHTML =
            `<p class='text-danger'>Failed to load suggestions.</p>`;
    }
}


async function addSuggestion(placeId) {
    const input = document.getElementById(`suggestion-input-${placeId}`);
    const content = input.value;

    if (!content.trim()) {
        alert("Please enter a suggestion.");
        return;
    }

    try {
        const response = await fetch("/Places/AddSuggestion", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ placeId: placeId, content: content })
        });

        const result = await response.json();
        if (response.ok && result.success) {
            alert(result.message || "Suggestion added successfully!");
            loadSuggestions(placeId); // Recargar sugerencias
            input.value = ""; // Limpiar input
        } else {
            alert(result.message || "Failed to add suggestion.");
        }
    } catch (error) {
        console.error("Error adding suggestion:", error);
        alert("An error occurred while adding your suggestion.");
    }
}



