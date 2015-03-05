#!/bin/sh
 
# Credits to:
#  - http://vstone.eu/reducing-vagrant-box-size/
#  - https://github.com/mitchellh/vagrant/issues/343
#  - https://gist.github.com/adrienbrault/3775253
 
# Remove APT cache
apt-get clean -y
apt-get autoclean -y
 
# Remove Linux headers
rm -rf /usr/src/linux-headers*
 
# Remove bash history
unset HISTFILE
rm -f /root/.bash_history
rm -f /home/vagrant/.bash_history
 
# Cleanup log files
find /var/log -type f | while read f; do echo -ne '' > $f; done;
 
# Zero free space to aid VM compression
echo "whiteout hda1"
dd if=/dev/zero of=/EMPTY bs=1M
rm -f /EMPTY
 
