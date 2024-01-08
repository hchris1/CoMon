# CoMon

A simple tool for Condition Monitoring.

> [!IMPORTANT]  
> This project was primarily developed for learning purposes. It is unknown how the future of this project will look like.

## Features

#### Create assets with packages to monitor pings, http(s) endpoints or results from the [CoMon Python Package](https://github.com/hchris1/Python-CoMon)

![create-asset](./_screenshots/create-asset.gif)

#### Create groups to cluster assets

![create-group](./_screenshots/create-group.gif)

#### See status results in table

![table](./_screenshots/table.gif)

#### Dashboards

![dashboard](./_screenshots/dashboard.gif)

## Installation

#### Docker

```bash
docker run -d --restart=always -p 8080:8080 -v comon:/data --name comon hchris1234/comon:latest
```

CoMon will be available at `http://localhost:8080`. You can login with the default user `admin` and password `123qwe`.

#### Home Assistant

Use the [Home Assistant Community Add-on: CoMon](https://github.com/hchris1/Addon-CoMon).
