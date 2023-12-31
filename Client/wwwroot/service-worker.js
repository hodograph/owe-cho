// In development, always fetch from the network and do not enable offline support.
// This is because caching would make development more difficult (changes would not
// be reflected on the first load after each change).
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