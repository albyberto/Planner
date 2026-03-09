window.blazorPushNotifications = {
    requestPermission: async function () {
        if (!("Notification" in window)) {
            console.warn("[Push] This browser does not support desktop notification");
            return false;
        }

        console.log("[Push] Current permission state:", Notification.permission);
        if (Notification.permission === "granted") {
            return true;
        }

        if (Notification.permission !== "denied") {
            try {
                const permission = await Notification.requestPermission();
                console.log("[Push] Permission request result:", permission);
                return permission === "granted";
            } catch (e) {
                console.error("[Push] Error requesting permission:", e);
                return false;
            }
        }

        console.warn("[Push] Notifications are denied by user or policy.");
        return false;
    },
    
    showNotification: function (title, options) {
        console.log("[Push] Triggering notification:", title);
        
        if (!("Notification" in window)) {
            console.warn("[Push] Notification API missing.");
            return;
        }

        if (Notification.permission === "granted") {
            try {
                const notification = new Notification(title, options);
                
                notification.onclick = function(event) {
                    event.preventDefault(); // prevent the browser from focusing the Notification's tab
                    window.focus();
                    notification.close();
                }
            } catch (e) {
                console.error("[Push] Failed to instantiate notification:", e);
            }
        } else {
            console.warn("[Push] Cannot show notification. Permission is:", Notification.permission);
        }
    }
};
