Powerstage for PowerPoint
==================================

"Stage display hack for PowerPoint"

## The idea behind this project

When giving a PowerPoint presentation, the laptop has a Presenter View whilst the main output is shown on a projector.

The concept of a stage display is an additional *third* display which faces the presenter/speaker (also known as a 'confidence monitor', and similar to the concept of a teleprompter).

Consider using a desktop PC instead of a laptop, where the desktop PC is operated by someone else from a desk *behind the audience*. The stage display now replaces the Presenter View for the speaker.

tldr; This project replicates the PowerPoint Presenter View onto a separate window called the 'stage display'.

![Stage display window](https://i.imgur.com/QVLBF2G.png)

There is the option to extract the slide text rather than render the image.

![Stage display window](https://i.imgur.com/O7ugit9.png)

![Overlay buttons designed to show over Presenter View](https://i.imgur.com/sEJDVDf.png)

![PowerPoint toolbar](https://i.imgur.com/W6wUTN0.png)

## Features

- Send messages to the stage display (mostly finished implementing)
- Hacky 'freeze projector display output' - works by taking a screenshot and showing it as topmost ;)

## Limitations

- Slides transitions won't show on the stage display.
- Videos won't show on the stage display.

## Current project status

This project is in an incomplete state (or I'd be selling it :P). Some features were only half-finished. e.g.

- Save/restore the window location (which display device)
- Overlay toolbar (pops up over the Presenter View) with buttons to send messages to the stage.
- Hacky 'show logo' view - works by showing an image and showing it as topmost.

## License

GPLv3