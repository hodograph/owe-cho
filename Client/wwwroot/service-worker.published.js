/* eslint-disable no-undef */
/* eslint-disable no-restricted-globals */
import { precacheAndRoute, createHandlerBoundToURL } from 'workbox-precaching';
import { NavigationRoute, registerRoute } from 'workbox-routing';
import { clientsClaim } from 'workbox-core';


// SETTINGS

// Claiming control to start runtime caching asap
clientsClaim();

// Use to update the app after user triggered refresh
//self.skipWaiting();

// PRECACHING

// Precache and serve resources from __WB_MANIFEST array
precacheAndRoute(self.__WB_MANIFEST);

// NAVIGATION ROUTING
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