VisionScreens Web Remote for PowerPoint*
==================================

*this is a working title.

**Work In Progress. No releases yet.**

Control PowerPoint from any phone, tablet or another computer over Wi-Fi.

## How to use (planned)
1. Start PowerPoint and open a presentation.
1. Launch this app (and probably allow it through the firewall when prompted).
2. Open the web browser on the client (e.g. the smartphone), and navigate to the address printed in the app UI.
3. Walk away from the PowerPoint laptop, hit buttons on the smartphone, and watch PowerPoint change slides over Wi-Fi :O

## Planned
- One single packed EXE including the web server, html/css/js and powerpoint API component - that's it!
- Send over the slide thumbnails to the client
- Handle powerpoint crashes/restarts reliably
- Listen to powerpoint events reliably, so client remote control always shows up-to-date info
- Handle network error cases, device sleeps disconnecting ws
- Make client UI responsive
- Add a Stage Display view (with Timers & stage messages!) - port it from c#/wpf, ideal for stage displays
- Separate endpoints
<webserver>/stage
<webserver>/remote
- Jump to any slide from a grid layout of slide thumbnails, or from a text-only view of the slides

## Technologies
- Websockets
- C#
- COM API (yay :/) like I did in https://github.com/navhaxs/wiimote-presenter-powerpoint, but that was from 6 years ago, so this will be a rewrite of the 'glue' code.
- Featuring a badly written HTTP 1.0 web server, in order to avoid using Windows' http.sys which depends on Administrator privileges!
- Mostly HTML/CSS/JS as the main client code rather than C#/WPF

## Note:
- This repo has been repurposed. It still has old and irrelevant C#/WPF powerpoint add-in code - a standalone app easier to develop! XD

## License
GPLv3

> A VisionScreens project.