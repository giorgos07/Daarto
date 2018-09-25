'use strict';

var gulp = require('gulp'),
    del = require('del'),
    npmDist = require('gulp-npm-dist');

var lib = './wwwroot/lib/';

gulp.task('clean:lib', function (callback) {
    del([
        lib + '**'
    ]).then(function () {
        callback();
    });
});

gulp.task('copy:libs', function (callback) {
    gulp.src(npmDist(), { base: './node_modules' })
        .pipe(gulp.dest(lib));

    callback();
});