document.addEventListener("submit", function (e) {
    if (e.target && e.target.id === "loginForm") {
        e.preventDefault();

        const formData = new FormData(e.target);
        fetch("/Account/Login", {
            method: "POST",
            body: formData
        })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                   
                    // Encuentra y cierra el modal
                    const modalElement = document.getElementById('authModal');
                    const modal = bootstrap.Modal.getInstance(modalElement);
                    if (modal) {
                        modal.hide();
                        document.getElementById('authModalBody').innerHTML = "";
                    }

                    // Redirige al usuario a la página principal
                    location.href = data.redirectUrl || "/";
                } else {
                    // Mostrar error en el formulario
                    document.getElementById("loginError").innerText = data.error || "Invalid credentials.";
                    document.getElementById("loginError").style.display = "block";
                }
            })
            .catch(error => {
                console.error("Error during login:", error);
                document.getElementById("loginError").innerText = "An unexpected error occurred.";
                document.getElementById("loginError").style.display = "block";
            });
    }
});

function handleFavoritesClick() {
    fetch('/Places/GetFavorites', { method: 'GET' })
        .then(response => {
            if (response.status === 401) {
                // Usuario no autenticado - abrir modal de login
                loadLogin();
                const modal = new bootstrap.Modal(document.getElementById('authModal'));
                modal.show();
                return; // Termina aquí si no está autenticado
            }
            return response.text();
        })
        .then(html => {
            if (html) {
                document.getElementById('favoritesModalBody').innerHTML = html;
                const modal = new bootstrap.Modal(document.getElementById('favoritesModal'));
                modal.show();
            }
        })
        .catch(error => {
            console.error("Error fetching favorites:", error);
            alert("An unexpected error occurred.");
        });
}

function loadLogin() {
    fetch('/Account/Login')
        .then(response => response.text())
        .then(html => {
            document.getElementById('authModalLabel').innerText = 'Login';
            document.getElementById('authModalBody').innerHTML = html;
        })
        .catch(error => {
            console.error("Error loading login form:", error);
            alert("Failed to load the login form.");
        });
}

function loadRegister() {
    fetch('/Account/Register')
        .then(response => response.text())
        .then(html => {
            document.getElementById('authModalLabel').innerText = 'Register';
            document.getElementById('authModalBody').innerHTML = html;
        });
}

function showFeedbackMessage(type, message) {
    const container = document.getElementById("feedbackMessage");
    container.innerHTML = `
        <div class="alert alert-${type === "success" ? "success" : "danger"} alert-dismissible fade show" role="alert">
            <i class="bi ${type === "success" ? "bi-check-circle" : "bi-exclamation-triangle"}"></i>
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
        </div>
    `;
    setTimeout(() => container.innerHTML = "", 5000); // Desaparece después de 5 segundos
}
