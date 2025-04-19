// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).ready(function () {
    console.log("site.js loaded");

    // Get the session token from the hidden input
    var initialSessionToken = $('#sessionToken').val();
    console.log("Initial session token:", initialSessionToken);

    // Force reload on back/forward navigation to bypass bfcache
    $(window).on("pageshow", function (event) {
        if (event.originalEvent.persisted) {
            console.log("Page restored from bfcache, reloading");
            window.location.reload();
        }
    });

    // Disable bfcache on unload
    $(window).on("unload", function () {
        // Empty handler to prevent bfcache
    });

    // Function to check authentication and session
    function checkAuthentication() {
        console.log("Checking authentication...");
        $.ajax({
            url: '/Home/CheckAuthentication',
            type: 'GET',
            success: function (data) {
                console.log("CheckAuthentication response:", data);
                if (!data.isAuthenticated) {
                    console.log("User not authenticated, redirecting to login");
                    window.location.href = '/Home/Login';
                    return;
                }

                // Validate session token
                $.ajax({
                    url: '/Home/GetSessionId',
                    type: 'GET',
                    success: function (sessionData) {
                        console.log("Current session ID:", sessionData.sessionId);
                        if (sessionData.sessionId !== initialSessionToken) {
                            console.log("Session token mismatch, redirecting to login");
                            window.location.href = '/Home/Login';
                        }
                    },
                    error: function () {
                        console.log("Session ID check error, redirecting to login");
                        window.location.href = '/Home/Login';
                    }
                });
            },
            error: function (xhr, status, error) {
                console.log("CheckAuthentication error:", error, "redirecting to login");
                window.location.href = '/Home/Login';
            }
        });
    }

    // Initial check on load
    checkAuthentication();

    // Periodic check every 5 seconds
    setInterval(checkAuthentication, 5000);

    // Session timeout logic
    let sessionTimeoutWarning = 25 * 60 * 1000;
    let sessionTimeout = 30 * 60 * 1000;
    let warningTimer, timeoutTimer, lastActivityTime = new Date().getTime();
    let isModalVisible = false;
    let countdownInterval, remainingTime = 5 * 60;

    function resetSessionTimers() {
        console.log("Resetting session timers");
        lastActivityTime = new Date().getTime();
        clearTimeout(warningTimer);
        clearTimeout(timeoutTimer);
        clearInterval(countdownInterval);
        if (isModalVisible) {
            isModalVisible = false;
            $('#sessionTimeoutModal').modal('hide');
        }
        warningTimer = setTimeout(showWarning, sessionTimeoutWarning);
        timeoutTimer = setTimeout(redirectToLogin, sessionTimeout);
        $.ajax({
            url: '/Home/RefreshSession',
            type: 'POST',
            headers: {
                "RequestVerificationToken": window.__RequestVerificationToken || ''
            },
            success: function () { console.log('Session refreshed'); },
            error: function (xhr, status, error) { console.error('Error refreshing session:', error); }
        });
    }

    function showWarning() {
        console.log("Showing session timeout warning");
        remainingTime = 5 * 60;
        updateCountdown();
        countdownInterval = setInterval(updateCountdown, 1000);
        $('#sessionTimeoutModal').modal('show');
        isModalVisible = true;
    }

    function updateCountdown() {
        if (remainingTime <= 0) {
            clearInterval(countdownInterval);
            redirectToLogin();
            return;
        }
        const minutes = Math.floor(remainingTime / 60);
        const seconds = remainingTime % 60;
        $('#countdown').text(`${minutes}:${seconds.toString().padStart(2, '0')}`);
        remainingTime--;
    }

    function redirectToLogin() {
        console.log("Session timeout, redirecting to login");
        window.location.href = '/Home/Login';
    }

    function continueSession() {
        console.log("Continuing session");
        resetSessionTimers();
    }

    const modalHtml = `
        <div class="modal fade" id="sessionTimeoutModal" tabindex="-1" role="dialog" aria-labelledby="sessionTimeoutModalLabel" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
            <div class="modal-dialog modal-dialog-centered">
                <div class="modal-content">
                    <div class="modal-header bg-warning">
                        <h5 class="modal-title" id="sessionTimeoutModalLabel">Session Timeout Warning</h5>
                    </div>
                    <div class="modal-body">
                        <p>Your session is about to expire due to inactivity.</p>
                        <p>You will be logged out in <span id="countdown">5:00</span> minutes.</p>
                        <p>Do you want to continue your session?</p>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" onclick="redirectToLogin()">Logout Now</button>
                        <button type="button" class="btn btn-primary" onclick="continueSession()">Continue Session</button>
                    </div>
                </div>
            </div>
        </div>`;
    $('body').append(modalHtml);

    $(document).on('click keypress scroll mousemove', resetSessionTimers);
    resetSessionTimers();
});