(function () {
    const authTabs = document.querySelectorAll(".auth-tab");
    const authPanels = document.querySelectorAll(".auth-panel");

    if (authTabs.length === 0) {
        return;
    }

    function showAuthPanel(name) {
        authTabs.forEach((tab) => {
            const selected = tab.id === `${name}-tab`;
            tab.classList.toggle("is-active", selected);
            tab.setAttribute("aria-selected", String(selected));
            tab.tabIndex = selected ? 0 : -1;
        });

        authPanels.forEach((panel) => {
            panel.hidden = panel.id !== `${name}-panel`;
        });
    }

    authTabs.forEach((tab) => {
        tab.addEventListener("click", () => showAuthPanel(tab.id.replace("-tab", "")));
    });

    document.querySelectorAll("[data-auth-target]").forEach((button) => {
        button.addEventListener("click", () => showAuthPanel(button.dataset.authTarget));
    });

    const password = document.querySelector("#Register_Password");
    const confirmPassword = document.querySelector("#Register_ConfirmPassword");

    if (!password || !confirmPassword) {
        return;
    }

    function validatePasswordMatch() {
        confirmPassword.setCustomValidity(
            confirmPassword.value === password.value ? "" : "Mật khẩu xác nhận không khớp."
        );
    }

    password.addEventListener("input", validatePasswordMatch);
    confirmPassword.addEventListener("input", validatePasswordMatch);
}());
