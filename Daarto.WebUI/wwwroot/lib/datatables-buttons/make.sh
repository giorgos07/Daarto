#!/bin/sh

DT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )/../.."
if [ "$1" = "debug" ]; then
	DEBUG="debug"
else
	OUT_DIR=$1
	DEBUG=$2
fi

# If not run from DataTables build script, redirect to there
if [ -z "$DT_BUILD" ]; then
	cd $DT_DIR/build
	./make.sh extension BUttons $DEBUG
	cd -
	exit
fi

# Change into script's own dir
cd $(dirname $0)

DT_SRC=$(dirname $(dirname $(pwd)))
DT_BUILT="${DT_SRC}/built/DataTables"
. $DT_SRC/build/include.sh

# Copy CSS
rsync -r css $OUT_DIR
css_frameworks buttons $OUT_DIR/css

# Copy images
#rsync -r images $OUT_DIR

# Copy JS
rsync -r js $OUT_DIR
js_compress $OUT_DIR/js/dataTables.buttons.js
js_frameworks buttons $OUT_DIR/js

js_compress $OUT_DIR/js/buttons.colVis.js
js_compress $OUT_DIR/js/buttons.html5.js
js_compress $OUT_DIR/js/buttons.flash.js
js_compress $OUT_DIR/js/buttons.print.js

# Copy and build examples
rsync -r examples $OUT_DIR
examples_process $OUT_DIR/examples

# SWF file flash export options
rsync -r swf $OUT_DIR

# Readme and license
cp Readme.md $OUT_DIR
cp License.txt $OUT_DIR

