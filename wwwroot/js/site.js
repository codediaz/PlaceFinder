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
                    // Cerrar modal y refrescar la página
                    const modal = bootstrap.Modal.getInstance(document.getElementById('authModal'));
                    modal.hide();
                    location.reload();
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

function loadLogin() {
    fetch('/Account/Login')
        .then(response => response.text())
        .then(html => {
            document.getElementById('authModalLabel').innerText = 'Login';
            document.getElementById('authModalBody').innerHTML = html;
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

function handleFavoritesClick() {
    fetch('/Places/GetFavorites', { method: 'GET' })
        .then(response => {
            if (response.status === 401) {
                // Usuario no autenticado
                alert("You need to log in or register to view your favorites.");
                loadLogin(); // Abre el modal de login
                return;
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
