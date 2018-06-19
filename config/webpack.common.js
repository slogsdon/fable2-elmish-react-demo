var fs = require("fs");
var path = require("path");
var fableUtils = require("fable-utils");
var HtmlWebpackPlugin = require("html-webpack-plugin");
var CleanWebpackPlugin = require("clean-webpack-plugin");

var packageJson = JSON.parse(fs.readFileSync(resolve("../package.json")).toString());
var errorMsg = "{0} missing in package.json";

var config = {
  entry: resolve(path.join("..", forceGet(packageJson, "fable.entry", errorMsg))),
  publicDir: resolve("../src/public"),
  buildDir: resolve("../dist"),
  rootDir: resolve("../"),
  nodeModulesDir: resolve("../node_modules"),
  indexHtmlTemplate: resolve("../src/index.html")
}

function resolve(filePath) {
  return path.join(__dirname, filePath)
}

function forceGet(obj, path, errorMsg) {
  function forceGetInner(obj, head, tail) {
    if (head in obj) {
      var res = obj[head];
      return tail.length > 0 ? forceGetInner(res, tail[0], tail.slice(1)) : res;
    }
    throw new Error(errorMsg.replace("{0}", path));
  }
  var parts = path.split(".");
  return forceGetInner(obj, parts[0], parts.slice(1));
}

function getModuleRules(isProduction) {
  var babelOptions = fableUtils.resolveBabelOptions({
    presets: [
      ["env", { "targets": { "browsers": "> 1%" }, "modules": false }]
    ],
  });

  return [
    {
      test: /\.fs(x|proj)?$/,
      use: {
        loader: "fable-loader",
        options: {
          babel: babelOptions,
          define: isProduction ? [] : ["DEBUG"]
        }
      }
    },
    {
      test: /\.js$/,
      exclude: /node_modules/,
      use: {
        loader: "babel-loader",
        options: babelOptions
      },
    }
  ];
}

function getPlugins(isProduction) {
  return [
    new HtmlWebpackPlugin({
      filename: path.join(config.buildDir, "index.html"),
      template: config.indexHtmlTemplate,
      minify: isProduction ? { collapseWhitespace: true } : false
    }),
    new CleanWebpackPlugin([ config.buildDir ], {
      root: config.rootDir,
      verbose: false,
    }),
  ];
}

module.exports = {
  resolve: resolve,
  config: config,
  getModuleRules: getModuleRules,
  getPlugins: getPlugins
};
