document.addEventListener("DOMContentLoaded", () => {
    const sidebar = document.getElementById("sidebar");
    const sidebarToggle = document.getElementById("sidebarToggle");
    const closeSidebar = document.getElementById("closeSidebar");
    const sidebarOverlay = document.getElementById("sidebarOverlay");

    function toggleSidebar() {
        sidebar.classList.toggle("show");
        if (sidebar.classList.contains("show")) {
            sidebarOverlay.style.display = "block";
            document.body.style.overflow = "hidden";

            setTimeout(() => {
                sidebarOverlay.style.opacity = "1";
            }, 10);
        } else {
            sidebarOverlay.style.opacity = "0";

            setTimeout(() => {
                sidebarOverlay.style.display = "none";
                document.body.style.overflow = "";
            }, 300);
        }
    }

    if (sidebarToggle) {
        sidebarToggle.addEventListener("click", toggleSidebar);
    }

    if (closeSidebar) {
        closeSidebar.addEventListener("click", toggleSidebar);
    }

    if (sidebarOverlay) {
        sidebarOverlay.addEventListener("click", toggleSidebar);
    }

    window.addEventListener("resize", () => {
        if (window.innerWidth >= 992 && sidebar.classList.contains("show")) {
            sidebar.classList.remove("show");
            sidebarOverlay.style.display = "none";
            document.body.style.overflow = "";
        }
    });

    const mobileCards = document.querySelectorAll(".mobile-item-card");
    if (mobileCards.length > 0) {
        mobileCards.forEach((card) => {
            card.addEventListener("touchstart", function () {
                this.style.transform = "translateY(-2px)";
                this.style.boxShadow = "0 5px 15px rgba(0, 0, 0, 0.1)";
            });

            card.addEventListener("touchend", function () {
                this.style.transform = "";
                this.style.boxShadow = "";
            });
        });
    }
});
