#!/bin/bash

# 1. the ssh key 
KEY_PATH="~/.ssh/ssh.key"

# 2. VPS Information
USER="root"      
IP="VPS IP"     
REMOTE_DIR="/opt/www/sites/yourdomain.com/" 

echo "Syncing output to VPS..."

rsync -avz --delete --rsync-path="sudo rsync" -e "ssh -i $KEY_PATH" output/ ${USER}@${IP}:${REMOTE_DIR}

echo "Deployment complete！"