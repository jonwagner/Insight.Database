# -*- mode: ruby -*-
# vi: set ft=ruby :

# ****************************************************
# Please note that this configuration has not been secured
# it relies on port-forwarding only on localhost/127.0.0.1 so there is no external network access
# DO NOT USE AS THE BASIS FOR ANY REAL SERVER
# ****************************************************

# All Vagrant configuration is done below. The "2" in Vagrant.configure
# configures the configuration version (we support older styles for
# backwards compatibility). Please don't change it unless you know what
# you're doing.
Vagrant.configure(2) do |config|

	# set up a reasonble size box
	config.vm.box = "ubuntu/trusty64" 
	config.vm.provider "virtualbox" do |v|
		v.name = "InsightTest"
		v.memory = 2048
	end  

	config.vm.provider "hyperv" do |v, override|
		override.vm.box = "ericmann/trusty64"
		v.vmname = "InsightTest"
		v.memory = 2048
	end

	# DB2 Port
	config.vm.network "forwarded_port", guest: 50000, host: 50000, host_ip: "127.0.0.1"
	# MySQL Port
	config.vm.network "forwarded_port", guest: 3306, host: 3306, host_ip: "127.0.0.1"
	# PostgreSQL Port
	config.vm.network "forwarded_port", guest: 5432, host: 5432, host_ip: "127.0.0.1"
	# Sybase Port
	config.vm.network "forwarded_port", guest: 5000, host: 5000, host_ip: "127.0.0.1"
	# Oracle Port
	config.vm.network "forwarded_port", guest: 1521, host: 1521, host_ip: "127.0.0.1"
  
	config.vm.provision "shell", inline: <<-SHELL
		# make sure that apt installs don't prompt for default passwords
		export DEBIAN_FRONTEND=noninteractive 
		export VAGRANTHOME=/vagrant/Vagrant

		# need a swapfile for a lot of these servers (Oracle won't install without this)
		if [ -f /swapfile ]
		then
			echo "/swapfile already exists"
		else
			sudo fallocate -l 1G /swapfile
			sudo chmod 600 /swapfile
			sudo mkswap /swapfile
			sudo swapon /swapfile
			sudo patch -b -N /etc/fstab < $VAGRANTHOME/fstab.diff
			sudo mount -a
		fi

		# set up kernel parameters - most of the dbs need shared memory
		sudo cp $VAGRANTHOME/60-databases.conf /etc/sysctl.d/60-databases.conf
		sudo service procps start
  
		# enable the partner repository in the source list
		# note that DB2 is only in the precise repository
		sudo add-apt-repository "deb http://archive.canonical.com precise partner"
  
		# update the packages
		sudo apt-get update
		sudo apt-get -y upgrade
		# aio1 is the low level block driver
		sudo apt-get install libaio1

		# install Sybase ASE
		# NOTE: if you download this file, you are agreeing to the SAP license terms
		if [ -f /etc/init.d/sybase ]
		then
			echo "Sybase is already installed"
		else
			# get sybase
			sudo apt-get -y install bsdtar
			mkdir ~/sybase
			wget -c -nv 'https://onedrive.live.com/download?cid=88E04946EED32E2B&resid=88E04946EED32E2B%216072&authkey=AOs45Ja-oNmyOps' -O - | bsdtar -xvf- -C ~/sybase

			# run setup
			chmod +x ~/sybase/setup.bin
			sudo ~/sybase/setup.bin -i silent -f $VAGRANTHOME/sybase.rsp

			# turn on remote access
			source /opt/sap/SYBASE.sh
			echo 'sp_configure "allow remote access", 1' | isql64 -Usa -P'Insight!!!Test'

			# set for start at boot
			sudo cp $VAGRANTHOME/sybase.init /etc/init.d/sybase 
			sudo update-rc.d sybase defaults

			# clean up the giant mess it leaves behind
			rm -rf ~/sybase
		fi
		
		# make system adjustments for oracle XE
		# from http://meandmyubuntulinux.blogspot.com/2012/05/installing-oracle-11g-r2-express.html
		sudo cp $VAGRANTHOME/chkconfig /sbin/chkconfig
		sudo chmod 755 /sbin/chkconfig
		sudo ln -s /usr/bin/awk /bin/awk 
		sudo mkdir /var/lock/subsys
		sudo touch /var/lock/subsys/listener

		# set up shared memory for oracle
		sudo cp /vagrant/Vagrant/oracle-shm /etc/init.d	
		sudo chmod 755 /etc/init.d/oracle-shm
		sudo update-rc.d oracle-shm defaults 01 99
		sudo service oracle-shm start

		# actually install oracle
		# NOTE: if you download this file, you are agreeing to the Oracle license terms
		if [ -f /etc/init.d/oracle-xe ]
		then
			echo "Oracle is already installed"
		else
			wget -nc -c -nv 'https://onedrive.live.com/download?cid=88E04946EED32E2B&resid=88E04946EED32E2B%215982&authkey=AOgB-v9pevoW8Os' -O ~/oracle-xe-11.deb
			sudo dpkg -i ~/oracle-xe-11.deb
			rm ~/oracle-xe-11.deb
		fi
		sudo -E /etc/init.d/oracle-xe configure responseFile=$VAGRANTHOME/oracle-xe.rsp		

		# install DB2 and set up the sample database
		sudo apt-get -y install db2exc
		echo "dasusr1:Insight!!!Test" | sudo chpasswd
		echo "db2inst1:Insight!!!Test" | sudo chpasswd
		echo "db2fenc1:Insight!!!Test" | sudo chpasswd
		export DB2HOME=/opt/ibm/db2/V9.7
		sudo -u db2inst1 $DB2HOME/bin/db2 deactivate database sample || true
		sudo -u db2inst1 $DB2HOME/bin/db2sampl -force
		
		# install MySQL
		sudo -E apt-get -y install mysql-server mysql-client
		mysql -u root -e 'CREATE DATABASE IF NOT EXISTS test; GRANT ALL ON test.* to root;'
		sudo -E cp $VAGRANTHOME/my.cnf /etc/mysql/my.cnf
		sudo service mysql restart
		
		# install PostgreSQL
		sudo add-apt-repository "deb http://apt.postgresql.org/pub/repos/apt/ trusty-pgdg main"
		wget --quiet -O - https://www.postgresql.org/media/keys/ACCC4CF8.asc | sudo apt-key add -
		sudo apt-get update
		sudo -E apt-get -y install postgresql
		export PGHOME=/etc/postgresql/9.6/main
		sudo -E cp $VAGRANTHOME/postgresql.conf $PGHOME/postgresql.conf
		sudo -E cp $VAGRANTHOME/pg_hba.conf $PGHOME/pg_hba.conf
		sudo service postgresql restart
		echo "postgres:Insight!!!Test" | sudo chpasswd

		# clean out the space
		sudo $VAGRANTHOME/cleandisk.sh
	SHELL
end
