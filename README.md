# Quixel Asset Downloader
Quixel Asset Downloader is a command-line tool written in .NET that interacts with the Quixel Megascans API to automate the process of gathering metadata, downloading assets in your chosen resolution, and extracting them for use in your projects. This tool simplifies asset management by automating tedious manual tasks, allowing you to quickly and efficiently integrate Quixel Megascans into your workflow.

## Features
- Fetch all Asset Metadata: Queries the Quixel Megascans API and retrieves metadata for assets, which is saved as JSON files.
- Download all Assets: Downloads Quixel Megascans assets based on the resolution you choose (2K, 4K, 8K).
- Extract all Assets: Automatically extracts downloaded ZIP files for easy access.
- Flexible Resolution Choices: Download assets in the resolution that fits your project requirements.

## First things first!
The downloader only works if you have the assets in your quixel account. @ the time of writing this downloader there was a js file created by someone else, which would "automate" the asset adding page by page on the quixel website. This js script can be found in the assets folder. Just login to the quixel website, go to the assets pages, (around 95 of them if i remember correctly) and run the js file in the browserconsole.

### Note: i think you can add all assets to your quixel account with a single click now. So no need for the js script.



## Installation
#### Clone or Download the Project:
- Clone the repository from GitHub:

```bash
git clone https://github.com/your-repo/quixel-asset-downloader.git
```

#### Build the Project:
Open the solution in Visual Studio and build the project. Ensure that all dependencies are installed and referenced correctly.

## Getting a bearer token and a authentication cookie
You will need a bearer token and a authentication cookie to use this tool.
You can provide both as a command-line argument.

### How to get the tokens
- Open your everyday browser (i used chrome for this)
- Goto https://quixel.com/megascans/home
- Login to your account (also works with en epic account)
- When you are back on the megascans/home page scroll down to some asset. (if you dont have any assets on your account you must run .... first)
- now hit F12 (open developer mode) and select the network tab
- When you have the networktab opened, click a asset to download. It doesnt matter if you actualy save the file or not.
- now in the network tab there should be an entry saying "extended", click on it
- In the headers of that entry are the bearer token and the cookie auth you need.
- Copy those values and pass them in the command line.
- Leave your browser open on that page you are on where you got the tokens. If you close the browser the session will be lost and the tool will not work.

## Requirements
To download all the quixel megascan assets you need space, and allot of it. I have downloaded every single asset in the highest possible resolution (8K for the most, there are 16K assets also, but you cannot download them) The 18871 assets take up 4,7TB extracted and don't forget the size of the zips before extracting. I have used a 8TB disk and it was full. After deleting the zips there is rougly 3TB left.

## Usage
The tool operates from the command line, and you can provide several arguments to control its behavior.

### Command-Line Arguments
Argument	Description	Example
-k or --api-key	Quixel API key (optional if set via environment variable).	-k your-api-key
-r or --resolution	Resolution of the assets to download (2K, 4K, 8K).	-r 4K
-o or --output	Directory where downloaded assets will be saved.	-o C:\Downloads\QuixelAssets
-a or --asset-id	Asset ID(s) to download. Can specify multiple assets by separating IDs with a comma.	-a asset_id_1,asset_id_2
-l or --list-assets	List available assets from the Quixel Megascans API (without downloading them).	-l
-m or --metadata-only	Fetch and save metadata only, without downloading the actual assets.	-m
-x or --extract	Automatically extract downloaded ZIP files.	-x
--help	Display help information and available command-line arguments.	--help

### Example Commands
Download and Extract Assets with Specific Resolution:

```bash
QuixelAssetDownloader.exe -k your-api-key -r 4K -a asset_id_1,asset_id_2 -o C:\Downloads\QuixelAssets -x
```
This command downloads the assets asset_id_1 and asset_id_2 in 4K resolution, saves them in the C:\Downloads\QuixelAssets directory, and extracts the downloaded ZIP files.

### Fetch Metadata Only (No Downloads):

```bash
QuixelAssetDownloader.exe -k your-api-key -a asset_id_1 -m -o C:\Metadata
```
This command fetches metadata for the asset asset_id_1 and saves it to the C:\Metadata directory without downloading the asset.

### List Available Assets:

```bash
QuixelAssetDownloader.exe -k your-api-key -l
```
This command lists all the available assets from the Quixel Megascans API using your API key.

## Workflow

### Retrieve Metadata:
The tool queries the Quixel Megascans API to retrieve asset metadata based on the provided asset IDs.
This metadata is saved as JSON files in the specified output directory.

### Download Assets:
The tool downloads the asset files in the chosen resolution (e.g., 2K, 4K, 8K).
Assets are saved as ZIP files in the output directory.

### Extract Assets:
If the -x or --extract flag is provided, the tool automatically extracts the downloaded ZIP files into their respective folders, making them ready to use.

### API Key Setup
You can provide the Quixel Megascans API Key as a command-line argument using -k or --api-key, or you can set it as an environment variable:

Setting the API Key as an Environment Variable:
On Windows, open the Command Prompt and run the following command:

```bash
setx QUIXEL_API_KEY "your-api-key"
```
You can then omit the -k argument when running the tool, and it will automatically use the environment variable.

## License
This project is licensed under the MIT License. Feel free to modify and distribute it as needed.


