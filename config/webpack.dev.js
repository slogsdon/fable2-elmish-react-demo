const path = require("path");
const webpack = require("webpack");
const common = require("./webpack.common");

// webpack-serve
const history = require("connect-history-api-fallback");
const convert = require("koa-connect");

console.log("Bundling for development...");

module.exports = {
  mode: "development",
  devtool: "source-map",
  entry: common.config.entry,
  output: {
    filename: '[name].js',
    path: common.config.buildDir,
    devtoolModuleFilenameTemplate: info =>
      path.resolve(info.absoluteResourcePath).replace(/\\/g, '/'),
  },
  module: {
    rules: common.getModuleRules()
  },
  plugins: common.getPlugins().concat([
      new webpack.HotModuleReplacementPlugin(),
      new webpack.NamedModulesPlugin()
  ]),
  resolve: {
    modules: [common.config.nodeModulesDir]
  },
};

module.exports.serve = {
  content: [common.config.rootDir],
  add: (app, middleware, options) => {
    const historyOptions = {
      // ... see: https://github.com/bripkens/connect-history-api-fallback#options
    };

    app.use(convert(history(historyOptions)));
  },
};
