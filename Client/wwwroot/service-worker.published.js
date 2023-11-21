self.addEventListener('fetch', () => { });

self.addEventListener('push', event => {
    const payload = event.data.json()
    self.registration.showNotification('SyncOwe', {
        body: payload.message,
        icon: 'SyncOwe512.png',
        vibrate: [100, 50, 100],
        data: { url: payload.url },
    })
})

self.addEventListener('notificationclick', event => {
    event.notification.close()
    event.waitUntil(clients.openWindow(event.notification.data.url))
})

// This assumes /index.html has been precached.
const navHandler = createHandlerBoundToURL('/index.html');
const navigationRoute = new NavigationRoute(navHandler, {
    denylist: [
        new RegExp('^/login'),
        new RegExp('^/logout'),
        new RegExp('^/.auth')
    ], // Also might be specified explicitly via allowlist
});
registerRoute(navigationRoute);