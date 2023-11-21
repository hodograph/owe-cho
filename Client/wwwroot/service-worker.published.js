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