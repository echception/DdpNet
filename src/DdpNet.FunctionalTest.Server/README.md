vagrant-meteor
==============

[Vagrant](http://www.vagrantup.com/) configuration for a [virtual machine](http://en.wikipedia.org/wiki/Virtual_machine)
that can run [Meteor](https://www.meteor.com/) apps. Can be used on Windows, Mac OS X or Linux.

## Getting started

The following instructions are optimized for Windows users.
If you have questions or problems after reading and doing the following instructions please open an issue and I will
try to address your issue.

### Installation

1. Install [Cygwin](http://www.cygwin.com/install.html) with the packages `openssh` and `rsync`.
2. Add the `<CYGWIN_INSTALL_DIR>/bin` folder to your [PATH](http://geekswithblogs.net/renso/archive/2009/10/21/how-to-set-the-windows-path-in-windows-7.aspx).
3. Install [VirtualBox](https://www.virtualbox.org/wiki/Downloads).
4. Install [Vagrant](http://www.vagrantup.com/downloads.html) (1.6.1 or newer is needed).
5. [Download this repository](https://github.com/Sanjo/vagrant-meteor/archive/master.zip) and unpack the downloaded zip file.
6. Install and start the Vagrant VM by executing `start.bat`.
It will take a little bit to download and install everything.
Read the next part "File Synchronisation" while it installs. ;-)

### File Synchronisation

The folder is synchronised to two places on the guest. Each synchronized folder has a special purpose.

#### Rsync - one way synchronisation for starting the Meteor app

The folder is synchronised with [Rsync](http://docs.vagrantup.com/v2/synced-folders/rsync.html) to the guest folder `/vagrant`.

* You must use this folder to start the Meteor app.
* You must use this folder for `mrt add` and `mrt update`.
* Changes won't be synchronized back to the host and will be deleted after the next sync.

#### Shared Folder - two way synchronisation for file editing

The folder is also synchronised with the [VirtualBox shared folder feature](https://www.virtualbox.org/manual/ch04.html#sharedfolders) to the guest folder `/vagrant2`.

* Use this folder to make changes in the guest that should be synchronized with the host.
* Use this folder to create an app with `mrt create`.
* After you added or updated smart packages you must copy the smart.json and smart.lock file
  from `/vagrant` to `/vagrant2` with (see [open issue](https://github.com/Sanjo/vagrant-meteor/issues/4)):
  
```bash
cp -f /vagrant/<MY_APP>/smart.* /vagrant2/<MY_APP>/
cp -f /vagrant/<MY_APP>/.meteor/packages /vagrant2/<MY_APP>/.meteor/
```
* Cannot be used to start the Meteor app.
* Cannot be used for `mrt add`, `mrt install` or `mrt update`

### Create your first Meteor app

Do the following steps in the SSH terminal that opened with executing the `start.bat` script.

```bash
# Create the app
cd /vagrant2
mrt create my-app --example leaderboard
# Add an Atomosphere package
cd /vagrant/my-app
mrt add bootstrap3-less
# Copy the changed smart.json and smart.lock to the host
cp -f /vagrant/my-app/smart.* /vagrant2/my-app/
# Start the app
mrt
```

Now open the URL `http://localhost:3000` in your browser. It should show you the leaderboard example.
Also look into the `vagrant-meteor` folder. It has automatically synchronised the my-app folder.
You can now make changes to the app that will automatically be synchronized with the Vagrant VM.

If you find this useful I would appreciate a [Gittip](https://www.gittip.com/Sanjo/).

## Further reading

Check out the [wiki](https://github.com/Sanjo/vagrant-meteor/wiki) for more details.
