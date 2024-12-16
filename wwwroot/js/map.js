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

    const iconUrl = place.categories?.[0]?.icon?.prefix + "bg_32" + place.categories?.[0]?.icon?.suffix ||
        "http://maps.google.com/mapfiles/ms/icons/red-dot.png";

    const marker = new google.maps.Marker({
        position: position,
        map: map,
        title: place.name, // Título del marcador
        icon: iconUrl || "http://maps.google.com/mapfiles/ms/icons/red-dot.png",
        placeId: place.fsq_id, // Guardar el ID del lugar
        distance: place.distance, // Guardar distancia
        timezone: place.timezone // Guardar zona horaria
    });

    // HTML del InfoWindow
    const infoWindowContent = `
                <div style="width: 350px; font-family: Arial, sans-serif;">
            <!-- Encabezado: Ícono, Nombre del Lugar y Calificación -->
            <div class="d-flex align-items-center mb-2">
                <img src="${iconUrl}" alt="icon" style="width: 50px; height: 50px; margin-right: 15px;" />
                <div>
                    <h6 class="fw-bold mb-1" style="margin: 0; color: #333;">${place.name}</h6>
                    <p class="small text-warning mb-0">
                        ★★★★☆ <span class="text-muted">(1)</span>
                    </p>
                    <span class="text-muted small">${place.categories?.[0]?.name || "N/A"}</span>
                </div>
            </div>

            <!-- Botones de acción -->
            <div class="d-flex justify-content-around mb-3">
                <button class="btn btn-outline-primary btn-sm" onclick="savePlaceFavorite('${place.fsq_id}', '${place.name}', ${place.distance}, '${place.timezone}')">
                    <i class="bi bi-heart"></i> Save
                </button>
                <button class="btn btn-outline-secondary btn-sm">
                    <i class="bi bi-geo-alt"></i> Directions
                </button>
                <button class="btn btn-outline-secondary btn-sm">
                    <i class="bi bi-share"></i> Share
                </button>
            </div>

            <!-- Detalles del lugar -->
            <div class="small mb-3">
                <p class="mb-1"><i class="bi bi-geo-alt-fill text-primary"></i> Address: ${place.location?.formatted_address || "N/A"}</p>
                <p class="mb-1"><i class="bi bi-rulers text-primary"></i> Distance: ${place.distance} meters</p>
                <p class="mb-0"><i class="bi bi-clock text-primary"></i> Timezone: ${place.timezone || "N/A"}</p>
            </div>

            <hr class="my-2"/>

            <!-- Sugerencias -->
            <h6 class="fw-bold mb-2">Suggestions</h6>
            <div id="suggestions-container-${place.fsq_id}" class="border rounded p-2 mb-2" style="max-height: 120px; overflow-y: auto;">
                <p class="text-muted small mb-0">Loading suggestions...</p>
            </div>

            <!-- Formulario para agregar sugerencias -->
            <textarea id="suggestion-input-${place.fsq_id}" class="form-control form-control-sm mb-2" rows="2" placeholder="Write your suggestion here..."></textarea>
            <button class="btn btn-primary btn-sm w-100" onclick="addSuggestion('${place.fsq_id}')">
                <i class="bi bi-chat-dots"></i> Submit Suggestion
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

    marker.placeDetails = {
        placeId: place.fsq_id,
        title: place.name,
        distance: place.distance,
        timezone: place.timezone
    };

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

async function savePlaceFavorite(placeId, name, distance, timezone) {
    try {
        const response = await fetch(`/Places/SavePlaceFavorite`, {
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
    const content = document.getElementById(`suggestion-input-${placeId}`).value;

    if (!content) {
        alert("Please enter a suggestion.");
        return;
    }

    // Buscar el marcador correspondiente usando placeId
    const marker = markers.find(m => m.placeDetails.placeId === placeId);

    if (!marker) {
        console.error("No marker found for placeId:", placeId);
        alert("Error: Place details not found.");
        return;
    }

    // Primero guardar el lugar usando savePlace
    const savePlaceResponse = await fetch(`/Places/SavePlace`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            placeId: placeId,
            name: marker.placeDetails.title,
            distance: marker.placeDetails.distance || 0,
            timezone: marker.placeDetails.timezone || "N/A"
        })
    });

    const saveResult = await savePlaceResponse.json();

    if (!saveResult.success) {
        alert("Failed to save place before adding suggestion.");
        return;
    }

    // Ahora enviar la sugerencia
    try {
        const response = await fetch("/Places/AddSuggestion", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                placeId: placeId,
                content: content
            })
        });

        const result = await response.json();

        if (result.success) {
            alert("Suggestion added successfully!");
            loadSuggestions(placeId); // Recargar sugerencias
        } else {
            alert(result.message || "Failed to add suggestion.");
        }
    } catch (error) {
        console.error("Error adding suggestion:", error);
        alert("An error occurred while adding your suggestion.");
    }
}



