#!/usr/bin/env bash
# Cloud Agent install script for the Octopus Energy .NET client.
set -euo pipefail

REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
DOTNET_INSTALL_DIR="/usr/share/dotnet"

install_sdk_channel() {
    local channel="$1"
    curl -fsSL https://dot.net/v1/dotnet-install.sh -o /tmp/dotnet-install.sh
    chmod +x /tmp/dotnet-install.sh
    sudo /tmp/dotnet-install.sh --channel "$channel" --install-dir "$DOTNET_INSTALL_DIR"
}

install_sdk_channel "10.0"
install_sdk_channel "8.0"

sudo ln -sf "$DOTNET_INSTALL_DIR/dotnet" /usr/local/bin/dotnet

export DOTNET_CLI_TELEMETRY_OPTOUT=1
export DOTNET_NOLOGO=1

dotnet --info
dotnet restore "${REPO_ROOT}/OctopusEnergy.slnx"
