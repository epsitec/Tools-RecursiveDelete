# Tools-RecursiveDelete

This tool takes a single argument on the command line:

```
RecursiveDelete foo\bar\node_modules
```

and it will delete the specified folder and all of its contents. In order to
walk around limitations in the Windows API, we rename the folders to `@` on
the way down the tree in order to shorten the path names, so that we don't
get errors because the path exceeds 250.

## What's the matter?

When working with `npm` it is not unusal to get `node_modules` directory
trees which are very deep, with paths reaching lengths of 400 or more
characters (this is a [known issue](https://github.com/joyent/node/issues/6960)).

Trying to delete these folders with the _Windows Explorer_  results in
error messages complaining about the fact that the _path is too long_ and
deletion becomes a nightmare.

To circumvent this problem, I've written this tiny naive tool which just
walks the folder structure, renames every folder to a one-character length
name while descending the tree, and then deletes everything while moving
back up the tree.

## I'm left with a folder called @

Sometimes, folders or files are in use (usually by the `explorer` process,
but this might also be the antivirus kicking in) and the tool cannot delete
or rename some folders or files.

Usually, the resulting folder structure has already been shortened enough so
that you can simply delete the remaining garbage with the _Windows Explorer_
